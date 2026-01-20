using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Models;
using PhoneStore.Data;
using Microsoft.EntityFrameworkCore;

namespace PhoneStore.Controllers;

public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;
    private readonly ApplicationDbContext _context;

    public ProductController(ILogger<ProductController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 6, int sort = 0, string? categoryIds = null, decimal? minPrice = null, decimal? maxPrice = null, string? keyword = null    )
    {
        //Chỉ lấy sản phẩm đang hoạt động
        var query = _context.Products.Where(p => p.status == 1);

         // Tìm kiếm theo từ khóa
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();
            query = query.Where(p =>
                p.product_name.Contains(keyword) ||
                p.brand.Contains(keyword)
            );
        }

         // Lấy danh sách category để hiển thị sidebar
        ViewBag.Categories = await _context.Categories
            .Where(c => c.status == 1)
            .ToListAsync();
       
        List<int> selectedCategoryIds = new();
        if (!string.IsNullOrEmpty(categoryIds))
        {
            selectedCategoryIds = categoryIds
                .Split(',')
                .Select(int.Parse)
                .ToList();
        }
        ViewBag.SelectedCategoryIds = selectedCategoryIds;

        // Filter theo nhiều category
        if (selectedCategoryIds.Any())
        {
            query = query.Where(p => selectedCategoryIds.Contains(p.category_id));
        }

        //Filter theo khoảng giá
        if (minPrice.HasValue)
            query = query.Where(p => p.price >= minPrice.Value);
        if (maxPrice.HasValue)
            query = query.Where(p => p.price <= maxPrice.Value);

        //Sắp xếp theo giá
        switch (sort)
        {
            case 1: // Price Low -> High
                query = query.OrderBy(p => p.price);
                break;
            case 2: // Price High -> Low
                query = query.OrderByDescending(p => p.price);
                break;
            default: // Popular
                query = query.OrderByDescending(p => p.sold_count).ThenByDescending(p => p.created_at);
                break;
        }

        //Tổng số sản phẩm (sau khi filter)
        int totalProducts = await query.CountAsync();

        //Tổng số trang
        int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

        //Lấy dữ liệu theo trang
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Top selling sidebar lấy 3 sản phẩm bán chạy nhất
        ViewBag.TopSellingProducts = await _context.Products
            .Where(p => p.status == 1)
            .OrderByDescending(p => p.sold_count)
            .Take(3)
            .ToListAsync();

        //Truyền dữ liệu ra View
        ViewBag.CurrentPage = page;      // Trang hiện tại
        ViewBag.TotalPages  = totalPages; // Tổng trang
        ViewBag.PageSize    = pageSize;   // Số SP / trang
        ViewBag.Sort        = sort;       // Trạng thái sort
        ViewBag.CategoryIds = categoryIds; // Danh sách category đã chọn
        ViewBag.MinPrice    = minPrice ?? 0;
        ViewBag.MaxPrice    = maxPrice ?? 50000000;
        ViewBag.Keyword     = keyword;  // Từ khóa tìm kiếm

        return View(products);
    }

    // ACTION DETAILS NTBinh 19/01
    public async Task<IActionResult> Details(int id)
    {
        // 1. Lấy thông tin sản phẩm theo id
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.product_id == id && p.status == 1);

        if (product == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
            return RedirectToAction("Index");
        }

        // 2. Lấy danh sách Review (Status = 1 là hiện)
        // Kết hợp bảng Users để lấy tên người bình luận
        var reviews = await _context.Reviews
            .Where(r => r.product_id == id && r.status == 1) 
            .OrderByDescending(r => r.created_at)
            .Join(_context.Users,
                  r => r.user_id,
                  u => u.user_id,
                  (r, u) => new ReviewViewModel // Dùng ViewModel đã tạo ở bước trước
                  {
                      Review = r,
                      UserFullName = u.full_name
                  })
            .ToListAsync();

        // 3. Tính toán thống kê review
        ViewBag.ListReviews = reviews;
        ViewBag.TotalReviews = reviews.Count;
        ViewBag.AverageRating = reviews.Any() ? reviews.Average(x => x.Review.rating) : 0;

        // 4. Lấy danh sách sản phẩm liên quan
        // Tăng lượt xem sản phẩm
        product.view_count += 1;
        await _context.SaveChangesAsync();

        // Lấy danh sách sản phẩm liên quan (cùng category, khác id)
        var relatedProducts = await _context.Products
            .Where(p => p.category_id == product.category_id && p.product_id != id && p.status == 1)
            .Take(4)
            .ToListAsync();

        ViewBag.RelatedProducts = relatedProducts;

        // === LẤY REVIEWS TỪ DATABASE ===
        // Lấy danh sách reviews kèm thông tin user
        var reviewsWithUser = await (from r in _context.Reviews
                                     join u in _context.Users on r.user_id equals u.user_id
                                     where r.product_id == id && r.status == 1
                                     orderby r.created_at descending
                                     select new ReviewWithUser
                                     {
                                         review_id = r.review_id,
                                         product_id = r.product_id,
                                         user_id = r.user_id,
                                         rating = r.rating,
                                         comment = r.comment,
                                         created_at = r.created_at,
                                         user_name = u.full_name ?? "Anonymous",
                                         user_email = u.email ?? ""
                                     }).ToListAsync();

        // Tính toán thống kê rating
        var allReviews = await _context.Reviews
            .Where(r => r.product_id == id && r.status == 1)
            .ToListAsync();

        var ratingSummary = new RatingSummary
        {
            TotalReviews = allReviews.Count,
            AverageRating = allReviews.Any() ? (decimal)allReviews.Average(r => r.rating) : 0,
            FiveStars = allReviews.Count(r => r.rating == 5),
            FourStars = allReviews.Count(r => r.rating == 4),
            ThreeStars = allReviews.Count(r => r.rating == 3),
            TwoStars = allReviews.Count(r => r.rating == 2),
            OneStar = allReviews.Count(r => r.rating == 1)
        };

        // Tạo ViewModel
        var viewModel = new ProductDetailsViewModel
        {
            Product = product,
            Reviews = reviewsWithUser,
            RatingSummary = ratingSummary
        };

        return View(viewModel);
    }

    // ACTION ADD REVIEW NTBinh 19/01
    [HttpPost]
    public async Task<IActionResult> AddReview(int product_id, string comment, int rating)
    {
        // Kiểm tra đăng nhập qua Session
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            // Nếu chưa đăng nhập, chuyển hướng sang trang Login
            return RedirectToAction("Login", "Auth");
        }

        // Tạo đối tượng Review mới
        var review = new Review
        {
            product_id = product_id,
            user_id = userId.Value,
            rating = rating,
            comment = comment ?? "", // Tránh null
            created_at = DateTime.Now,
            status = 1 // 1 (byte) = Hiện, 0 = Ẩn
        };

        // Lưu vào database
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        // Load lại trang chi tiết sản phẩm
        return RedirectToAction("Details", new { id = product_id });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}