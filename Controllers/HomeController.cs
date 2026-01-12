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
                .ToListAsync()
        };
        
        _logger.LogInformation($"Found {viewModel.NewProducts.Count} new products");
        _logger.LogInformation($"Found {viewModel.TopSelling.Count} top selling products");
        
        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
