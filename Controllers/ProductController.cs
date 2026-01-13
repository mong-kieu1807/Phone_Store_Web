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

     public async Task<IActionResult> Index(int page = 1, int pageSize = 3, int sort = 0)
    {
        //Chỉ lấy sản phẩm đang hoạt động
        var query = _context.Products.Where(p => p.status == 1);

        switch (sort)
        {
            case 1: // Price Low -> High
                query = query.OrderBy(p => p.price);
                break;
            case 2: // Price High -> Low
                query = query.OrderByDescending(p => p.price);
                break;
            default: // Popular
                query = query.OrderByDescending(p => p.created_at);
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

        //Truyền dữ liệu ra View
        ViewBag.CurrentPage = page;      // Trang hiện tại
        ViewBag.TotalPages  = totalPages; // Tổng trang
        ViewBag.PageSize    = pageSize;   // Số SP / trang
        ViewBag.Sort        = sort;       // Trạng thái sort

        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        // Lấy thông tin sản phẩm theo ID
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.product_id == id && p.status == 1);

        // Nếu không tìm thấy sản phẩm, trả về 404
        if (product == null)
        {
            return NotFound();
        }

        // Lấy sản phẩm liên quan (cùng brand hoặc cùng category, khác product_id)
        var relatedProducts = await _context.Products
            .Where(p => p.status == 1 
                     && p.product_id != id 
                     && (p.brand == product.brand || p.category_id == product.category_id))
            .OrderByDescending(p => p.created_at)
            .Take(4)
            .ToListAsync();

        // Truyền dữ liệu vào ViewBag
        ViewBag.RelatedProducts = relatedProducts;

        return View(product);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
