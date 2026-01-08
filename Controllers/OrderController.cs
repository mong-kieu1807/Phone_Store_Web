using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Models; // Bắt buộc có dòng này để dùng class CartItem

namespace PhoneStore.Controllers
{
	public class OrderController : Controller
	{
		private readonly ILogger<OrderController> _logger;

		public OrderController(ILogger<OrderController> logger)
		{
			_logger = logger;
		}

		// 1. Action cho trang Thanh Toán (Checkout)
		public IActionResult Checkout()
		{
			// Tạo dữ liệu giả lập (Mô phỏng lấy từ Database bảng Products)
			var gioHang = LayDuLieuGiaLap();

			// Tính tổng tiền cho View hiển thị
			ViewBag.TongTienDonHang = gioHang.Sum(p => p.Total);

			return View(gioHang);
		}

		// 2. Action cho trang Giỏ Hàng (Cart) - MỚI THÊM VÀO
		public IActionResult Cart()
		{
			// Tái sử dụng dữ liệu giả lập
			var gioHang = LayDuLieuGiaLap();

			// Tính tổng tiền
			ViewBag.TongTien = gioHang.Sum(p => p.Total);

			return View(gioHang);
		}

		// Hàm riêng để tạo dữ liệu giả (Dùng chung cho cả 2 trang)
		private List<CartItem> LayDuLieuGiaLap()
		{
			return new List<CartItem>
			{
				new CartItem {
					ProductId = 1,
					ProductName = "iPhone 15 Pro Max", 
                    Price = 30000000,                  
                    Quantity = 1,
					Image = "product01.png"           
                },
				new CartItem {
					ProductId = 2,
					ProductName = "Samsung Galaxy S24",
					Price = 20000000,
					Quantity = 2,
					Image = "product02.png"
				}
			};
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}