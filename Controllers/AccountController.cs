using Microsoft.AspNetCore.Mvc;
using PhoneStore.Data;
using PhoneStore.Models;

namespace PhoneStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Chạy cái này để hiện form nhập liệu
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // 2. Chạy cái này khi người dùng bấm nút "Xác nhận"
        [HttpPost]
        public IActionResult ForgotPassword(string username, string email, string phone, string newPassword)
        {
            // Tìm user trong Database khớp cả 3 thông tin
            var user = _context.Users.FirstOrDefault(u =>
                u.username == username &&
                u.email == email &&
                u.phone == phone);

            // Nếu không tìm thấy ai
            if (user == null)
            {
                ViewBag.Error = "Thông tin xác minh không chính xác!";
                return View(); // Trả về form để nhập lại
            }

            // Nếu tìm thấy -> Đổi mật khẩu
            user.password = newPassword;
            _context.SaveChanges(); // Lưu xuống SQL

            ViewBag.Success = "Đổi mật khẩu thành công! Mật khẩu mới là: " + newPassword;
            return View();
        }
    }
}