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

    public async Task<IActionResult> Details(int id)
    {
        // Lấy thông tin sản phẩm theo id
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.product_id == id && p.status == 1);

        if (product == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
            return RedirectToAction("Index");
        }

        // Lấy danh sách sản phẩm liên quan (cùng category, khác id)
        var relatedProducts = await _context.Products
            .Where(p => p.category_id == product.category_id && p.product_id != id && p.status == 1)
            .Take(4)
            .ToListAsync();

        ViewBag.RelatedProducts = relatedProducts;

        return View(product);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
