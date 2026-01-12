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
		public async Task<IActionResult> Login(string username, string password)
		{
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				ViewBag.Error = "Vui lòng nhập tài khoản và mật khẩu";
				return View();
			}

			try
			{
				var user = await _context.Users
					.FirstOrDefaultAsync(u => u.username == username && u.password == password && u.status == 1);

				if (user != null && user.role == "Admin")
				{
					// Lưu thông tin user vào session
					HttpContext.Session.SetString("AdminLoggedIn", user.user_id.ToString());
					HttpContext.Session.SetString("AdminUsername", user.username);
					HttpContext.Session.SetString("AdminFullName", user.full_name);
					
					return RedirectToAction("Index", "Home");
				}
				else if (user != null && user.role != "Admin")
				{
					ViewBag.Error = "Bạn không có quyền truy cập trang quản trị.";
				}
				else
				{
					ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
				}
			}
			catch (Exception ex)
			{
				ViewBag.Error = "Lỗi: " + ex.Message;
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