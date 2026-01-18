// CHNhu
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;

namespace PhoneStore.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class HomeController : Controller
	{
		private readonly ApplicationDbContext _context;

		public HomeController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: HomeController
		public async Task<IActionResult> Index()
		{
			// Lấy thống kê tổng quan
			ViewBag.TotalProducts = await _context.Products.CountAsync();
			ViewBag.TotalOrders = await _context.Bills.CountAsync();
			ViewBag.TotalUsers = await _context.Users.CountAsync();
			ViewBag.TotalRevenue = await _context.Bills
				.Where(b => b.status == 1)
				.SumAsync(b => (decimal?)b.total_amount) ?? 0;

			// Đơn hàng gần đây
			ViewBag.RecentOrders = await _context.Bills
				.OrderByDescending(b => b.created_at)
				.Take(5)
				.ToListAsync();

			// Sản phẩm sắp hết hàng
			ViewBag.LowStockProducts = await _context.Products
				.Where(p => p.stock_quantity < 10)
				.OrderBy(p => p.stock_quantity)
				.Take(5)
				.ToListAsync();

			return View();
		}

	}
}
