using Microsoft.AspNetCore.Mvc;
using PhoneStore.Data;
using PhoneStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace PhoneStore.Controllers
{
 
   
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

     
        //NTNguyen
        ///Đăng kí
        /// Hiển thị trang đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            // Nếu đã đăng nhập rồi thì không cho vào trang đăng ký nữa
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        /// Xử lý đăng ký tài khoản mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User nguoiDung)
        {
            try
            {
                // B1: Kiểm tra username đã tồn tại chưa
                var daTonTaiUsername = await _context.Users
                    .AnyAsync(u => u.username == nguoiDung.username);
                
                if (daTonTaiUsername)
                {
                    ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                    return View(nguoiDung);
                }

                // B2: Kiểm tra email đã được dùng chưa
                var daTonTaiEmail = await _context.Users
                    .AnyAsync(u => u.email == nguoiDung.email);
                
                if (daTonTaiEmail)
                {
                    ViewBag.Error = "Email đã được sử dụng!";
                    return View(nguoiDung);
                }

                // B3: Tạo user mới
                nguoiDung.password = MaHoaMatKhau(nguoiDung.password); // Hash mật khẩu
                nguoiDung.role = "User";                               // Mặc định là User
                nguoiDung.status = 1;                                  // Kích hoạt
                nguoiDung.created_at = DateTime.Now;
                nguoiDung.updated_at = DateTime.Now;

                // B4: Lưu vào database
                _context.Users.Add(nguoiDung);
                await _context.SaveChangesAsync();

                // B5: Thông báo thành công và chuyển đến trang đăng nhập
                TempData["Success"] = "🎉 Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi xảy ra: " + ex.Message;
                return View(nguoiDung);
            }
        }

        ///Đăng nhập
        /// Hiển thị trang đăng nhập
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Nếu đã đăng nhập rồi
            var maNguoiDung = HttpContext.Session.GetInt32("UserId");
            if (maNguoiDung != null)
            {
                var vaiTro = HttpContext.Session.GetString("UserRole");
                
                // Nếu là admin thì vào trang qtri
                if (vaiTro == "Admin")
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                
                // Nếu là user thì vào trchu
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

    
        /// Xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            // B1: Validate input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            try
            {
                // B2: Tìm user trong database
                var nguoiDung = await _context.Users
                    .FirstOrDefaultAsync(u => u.username == username && u.status == 1);

                if (nguoiDung == null)
                {
                    ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                    return View();
                }

                // B3: Kiểm tra mật khẩu
                var matKhauDaMaHoa = MaHoaMatKhau(password);
                bool matKhauDung = false;

                if (nguoiDung.password == matKhauDaMaHoa)
                {
                    // Password đã hash 
                    matKhauDung = true;
                }
                else if (nguoiDung.password == password)
                {
                    // Password chưa hash - Tự động hash lại cho lần sau
                    nguoiDung.password = matKhauDaMaHoa;
                    nguoiDung.updated_at = DateTime.Now;
                    await _context.SaveChangesAsync();
                    matKhauDung = true;
                }

                if (!matKhauDung)
                {
                    ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                    return View();
                }

                // B: Lưu thông tin vào Session
                HttpContext.Session.SetInt32("UserId", nguoiDung.user_id);
                HttpContext.Session.SetString("Username", nguoiDung.username);
                HttpContext.Session.SetString("FullName", nguoiDung.full_name ?? "User");
                HttpContext.Session.SetString("UserRole", nguoiDung.role ?? "User");
                HttpContext.Session.SetString("UserEmail", nguoiDung.email ?? "");

                // B5: Redirect theo role
                if (nguoiDung.role == "Admin")
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                
                // Quay lại trang cũ hoặc trang chủ
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi xảy ra: " + ex.Message;
                return View();
            }
        }

        
        /// Đxuat
        /// Đăng xuất và xóa Session
        public IActionResult Logout()
        {
            var vaiTro = HttpContext.Session.GetString("UserRole");
            
            // Xóa toàn bộ Session
            HttpContext.Session.Clear();
            
            // Nếu là admin thì vào trang qtri
            // Nếu là admin thì vào trang chu
            return RedirectToAction("Login");
        }

           private string MaHoaMatKhau(string matKhau)
        {
            using (var sha256 = SHA256.Create())
            {
                var mangByte = sha256.ComputeHash(Encoding.UTF8.GetBytes(matKhau));
                return Convert.ToBase64String(mangByte);
            }
        }
    }
}
//endNTNguyen
