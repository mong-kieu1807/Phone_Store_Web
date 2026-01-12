// AuthController cho Admin
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Data;
using PhoneStore.Models;
using Microsoft.EntityFrameworkCore;

namespace PhoneStore.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class AuthController : Controller
	{
		private readonly ApplicationDbContext _context;

		public AuthController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult Login()
		{
			// Nếu đã đăng nhập, redirect về trang admin
			if (HttpContext.Session.GetString("AdminLoggedIn") != null)
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		public IActionResult Login(string username, string password)
		{
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				ViewBag.Error = "Vui lòng nhập tài khoản và mật khẩu";
				return View();
			}

			// Dữ liệu tĩnh - Tài khoản admin mẫu
			if (username == "admin" && password == "admin123")
			{
				// Lưu thông tin user vào session
				HttpContext.Session.SetString("AdminLoggedIn", "1");
				HttpContext.Session.SetString("AdminUsername", "admin");
				HttpContext.Session.SetString("AdminFullName", "Quản trị viên");
				
				return RedirectToAction("Index", "Home");
			}
			else
			{
				ViewBag.Error = "Sai tài khoản hoặc mật khẩu. Dùng: admin/admin123";
			}

			return View();
		}

		[HttpGet]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login");
		}
	}
}
// endCHNhu