//NTNguyen
// AuthController cho Admin - chuyển đến AuthController chung
using Microsoft.AspNetCore.Mvc;

namespace PhoneStore.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class AuthController : Controller
	{
		/// <summary>
		/// Redirect trang Login của Admin về AuthController chung
		/// </summary>
		[HttpGet]
		public IActionResult Login()
		{
			// Nếu đã đăng nhập với role Admin
			var maNguoiDung = HttpContext.Session.GetString("UserId");
			var vaiTro = HttpContext.Session.GetString("UserRole");
			
			if (maNguoiDung != null && vaiTro == "Admin")
			{
				return RedirectToAction("Index", "Home", new { area = "Admin" });
			}
			
			// Redirect về trang login chung
			return RedirectToAction("Login", "Auth", new { area = "" });
		}

		///  Logout về AuthController chung
		
		[HttpGet]
		public IActionResult Logout()
		{
			return RedirectToAction("Logout", "Auth", new { area = "" });
		}
	}
}
//endNTNguyen