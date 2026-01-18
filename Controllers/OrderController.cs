// NTBinh 14/01/2026  
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Models;
using PhoneStore.Data;
using Microsoft.EntityFrameworkCore; // Cần cái này để dùng FindAsync
using System.Text.Json; // Cần cái này để xử lý Session
using PhoneStore.Helper; // Import để dùng [Authorize]

namespace PhoneStore.Controllers
{
    [PhoneStore.Helper.Authorize] // Yêu cầu đăng nhập cho toàn bộ controller
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        //HÀM HỖ TRỢ: Lấy/Lưu Giỏ hàng vào Session 
        private List<CartItem> LayGioHang()
        {
            var session = HttpContext.Session;
            string json = session.GetString("GioHang");
            if (string.IsNullOrEmpty(json))
                return new List<CartItem>();
            return JsonSerializer.Deserialize<List<CartItem>>(json);
        }

        private void LuuGioHang(List<CartItem> gioHang)
        {
            var session = HttpContext.Session;
            string json = JsonSerializer.Serialize(gioHang);
            session.SetString("GioHang", json);
        }

        // ACTION 1: THÊM VÀO GIỎ
        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound("Lỗi: Sản phẩm không tồn tại");

            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(p => p.ProductId == id);

            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                gioHang.Add(new CartItem
                {
                    ProductId = product.product_id,
                    ProductName = product.product_name,
                    Price = product.price,
                    Image = product.image,
                    Quantity = 1
                });
            }

            LuuGioHang(gioHang);
            return RedirectToAction("Cart");
        }

        // Action dành riêng cho nút "Mua ngay"
        public async Task<IActionResult> BuyNow(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(p => p.ProductId == id);

            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                gioHang.Add(new CartItem
                {
                    ProductId = product.product_id,
                    ProductName = product.product_name,
                    Price = product.price,
                    Image = product.image,
                    Quantity = 1
                });
            }

            LuuGioHang(gioHang);
            return RedirectToAction("Cart");
        }

        //ACTION 2: HIỂN THỊ GIỎ HÀNG CÓ PHÂN TRANG (PAGINATION)
        
        public IActionResult Cart(int page = 1)
        {
            var gioHang = LayGioHang();

            // 1. Tính tổng tiền của TOÀN BỘ giỏ hàng (để hiển thị Total)
            ViewBag.TongTien = gioHang.Sum(p => p.Total);

            // 2. Cấu hình phân trang
            int pageSize = 4; // Hiển thị 4 sản phẩm mỗi trang
            if (page < 1) page = 1;

            int totalItems = gioHang.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // 3. Cắt dữ liệu theo trang
            var pagedCart = gioHang.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // 4. Truyền thông tin trang qua ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            // Trả về danh sách đã cắt gọn
            return View(pagedCart);
        }

        //ACTION: XÓA TOÀN BỘ GIỎ HÀNG
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("GioHang");
            return RedirectToAction("Cart");
        }

        // ACTION 3: XÓA 1 SẢN PHẨM
        public IActionResult RemoveFromCart(int id)
        {
            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(p => p.ProductId == id);

            if (item != null)
            {
                gioHang.Remove(item);
                LuuGioHang(gioHang);
            }
            return RedirectToAction("Cart");
        }
        // Action Cập nhật số lượng từ ô input
        [HttpPost]
        public IActionResult UpdateCart(int id, int quantity)
        {
            // Lấy giỏ hàng từ Session
            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(p => p.ProductId == id);

            if (item != null)
            {
                // Gán số lượng mới từ ô input
                item.Quantity = quantity;

                // Nếu người dùng nhập số âm hoặc 0, tự động chỉnh về 1
                if (item.Quantity < 1) item.Quantity = 1;

                // Lưu lại Session
                LuuGioHang(gioHang);
            }

            // Load lại trang Giỏ hàng
            return RedirectToAction("Cart");
        }

        // ACTION 4: TRANG THANH TOÁN (Checkout)
        public IActionResult Checkout()
        {
            var gioHang = LayGioHang();
            if (gioHang.Count == 0) return RedirectToAction("Index", "Home");

            ViewBag.TongTienDonHang = gioHang.Sum(p => p.Total);
            return View(gioHang);
        }

        // ACTION 5: XỬ LÝ ĐẶT HÀNG (ĐÃ NÂNG CẤP: Tự tạo tài khoản)
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string FullName, string Phone, string Address, string Note, string Password, bool CreateAccount, string Email)
        {
            var gioHang = LayGioHang();
            if (gioHang.Count == 0) return RedirectToAction("Cart");

            // 1. XÁC ĐỊNH NGƯỜI DÙNG (USER ID)
            int userId = 2; // Mặc định là khách vãng lai (ID=2) nếu không tạo tài khoản

            // Nếu khách tích vào ô "Tạo tài khoản mới" (CreateAccount = true) và có nhập Mật khẩu
            if (CreateAccount == true && !string.IsNullOrEmpty(Password))
            {
                // Kiểm tra xem Username/Email đã tồn tại chưa (để tránh lỗi trùng)
                var checkUser = _context.Users.FirstOrDefault(u => u.email == Email); // Dùng Email làm Username luôn cho tiện

                if (checkUser == null)
                {
                    // Tạo User mới
                    var newUser = new User
                    {
                        username = Email, // Lấy email làm tên đăng nhập
                        password = Password, // Lưu mật khẩu (nên mã hóa MD5 nếu cần)
                        full_name = FullName,
                        email = Email,
                        phone = Phone,
                        address = Address,
                        role = "User",
                        status = 1,
                        created_at = DateTime.Now
                    };

                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync(); // Lưu vào SQL để sinh ra ID mới

                    userId = newUser.user_id; // LẤY ID CỦA NGƯỜI VỪA TẠO
                }
            }
            // Lấy user_id từ Session
            var userid = HttpContext.Session.GetInt32("UserId");
            if (userid == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            // 2. TẠO HÓA ĐƠN (BILL) VỚI USER ID VỪA CÓ
            var bill = new Bill
            {
                user_id = userId, // Dùng ID vừa xác định ở trên (không hardcode số 2 nữa)
                total_amount = gioHang.Sum(x => x.Total),
                shipping_status = "Chờ xử lý",
                payment_method_id = 1,
                created_at = DateTime.Now,
                status = 1,
            };

            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            // 3. TẠO CHI TIẾT HÓA ĐƠN
            foreach (var item in gioHang)
            {
                var detail = new BillDetail
                {
                    order_id = bill.order_id,
                    product_id = item.ProductId,
                    quantity = item.Quantity,
                    price = item.Price
                };
                _context.BillDetails.Add(detail);
            }
            await _context.SaveChangesAsync();

            // 4. XÓA GIỎ HÀNG & THÔNG BÁO
            HttpContext.Session.Remove("GioHang");

            return RedirectToAction("OrderSuccess");
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}