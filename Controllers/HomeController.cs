using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Models;
using PhoneStore.Data;
using Microsoft.EntityFrameworkCore;

namespace PhoneStore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeViewModel
        {
            // Lấy 10 sản phẩm mới nhất
            NewProducts = await _context.Products
                .Where(p => p.status == 1)
                .OrderByDescending(p => p.created_at)
                .Take(10)
                .ToListAsync(),

            // Lấy sản phẩm bán chạy nhất
            TopSelling = await _context.Products
                .Where(p => p.status == 1)
                .OrderByDescending(p => p.sold_count)
                .Take(10)
                .ToListAsync(),
            // Lấy sản phẩm giảm giá tốt nhất
            HotDeals = await _context.Products
                .Where(p => p.status == 1)
                .OrderByDescending(p => p.discount_percent)
                .Take(10)
                .ToListAsync(),
            // Lấy sản phẩm bán chạy nhất
            TopRating = await _context.Products
                .Where(p => p.status == 1)
                .OrderByDescending(p => p.rating)
                .Take(10)
                .ToListAsync(),
        };
        
        _logger.LogInformation($"Found {viewModel.NewProducts.Count} new products");
        _logger.LogInformation($"Found {viewModel.TopSelling.Count} top selling products");
        
        return View(viewModel);
    }

    public async Task<IActionResult> AboutUs(string? searchKeyword, int page = 1)
    {
        int pageSize = 6; // Số bài viết mỗi trang
        
        // Query blogs với tìm kiếm
        var blogsQuery = _context.Blogs.Where(b => b.status == 1);
        
        if (!string.IsNullOrWhiteSpace(searchKeyword))
        {
            blogsQuery = blogsQuery.Where(b => 
                b.title.Contains(searchKeyword) || 
                b.content.Contains(searchKeyword) || 
                b.summary.Contains(searchKeyword));
        }
        
        // Tổng số bài viết
        var totalBlogs = await blogsQuery.CountAsync();
        
        // Lấy blogs cho trang hiện tại
        var blogs = await blogsQuery
            .OrderByDescending(b => b.created_at)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        // Tính tổng số trang
        ViewBag.TotalPages = (int)Math.Ceiling(totalBlogs / (double)pageSize);
        ViewBag.CurrentPage = page;
        ViewBag.SearchKeyword = searchKeyword;
        
        return View(blogs);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
