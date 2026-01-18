//NTBinh 14/01/2026
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

        // Constructor: Tiêm DbContext để kết nối SQL
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- HÀM HỖ TRỢ: Lấy/Lưu Giỏ hàng vào Session ---
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

        
        // ACTION 1: THÊM VÀO GIỎ (Kết nối Database thật để lấy thông tin SP)
        
        public async Task<IActionResult> AddToCart(int id)
        {
            // 1. TRUY VẤN DỮ LIỆU THẬT TỪ SQL
            // Tìm sản phẩm có product_id bằng id gửi lên
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Lỗi: Sản phẩm không tồn tại trong Database");
            }

            // 2. XỬ LÝ GIỎ HÀNG (SESSION)
            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(p => p.ProductId == id);

            if (item != null)
            {
                item.Quantity++; // Đã có thì tăng số lượng
            }
            else
            {
                // Mapping: Lấy dữ liệu thật từ biến 'product' đổ vào giỏ
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

            // 3. CHUYỂN HƯỚNG
            return RedirectToAction("Cart");
        }
        // Action dành riêng cho nút "Mua ngay" -> Chuyển hướng luôn
        public async Task<IActionResult> BuyNow(int id)
        {
            // 1. Tìm sản phẩm
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // 2. Lấy giỏ hàng từ Session
            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(p => p.ProductId == id);

            // 3. Thêm hoặc tăng số lượng
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

            // 4. Lưu Session
            LuuGioHang(gioHang);

            // 5. QUAN TRỌNG: Chuyển hướng ngay lập tức sang trang Giỏ hàng
            return RedirectToAction("Cart");
        }

        // ACTION 2: HIỂN THỊ GIỎ HÀNG (Lấy dữ liệu thật từ Session)
        public IActionResult Cart()
        {
            var gioHang = LayGioHang();

            // Tính tổng tiền thật
            ViewBag.TongTien = gioHang.Sum(p => p.Total);

            return View(gioHang);
        }

        // ACTION 3: XÓA SẢN PHẨM
        
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

        
        // ACTION 4: TRANG THANH TOÁN (Checkout)
        public IActionResult Checkout()
        {
            var gioHang = LayGioHang();

            if (gioHang.Count == 0) return RedirectToAction("Index", "Home");

            ViewBag.TongTienDonHang = gioHang.Sum(p => p.Total);
            return View(gioHang);
        }

        // ACTION 5: XỬ LÝ ĐẶT HÀNG (Lưu vào Database thật: Bills, Bill_Details)
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string FullName, string Phone, string Address, string Note)
        {
            var gioHang = LayGioHang();
            if (gioHang.Count == 0) return RedirectToAction("Cart");

            // Lấy user_id từ Session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // 1. Tạo Bill (Dữ liệu thật từ Form + Session)
            var bill = new Bill
            {
                user_id = userId.Value,
                total_amount = gioHang.Sum(x => x.Total),
                shipping_status = "Chờ xử lý",
                payment_method_id = 1,
                created_at = DateTime.Now,
                status = 1
            };

            _context.Bills.Add(bill);
            await _context.SaveChangesAsync(); // Lưu xuống SQL để sinh order_id

            // 2. Tạo Bill Detail
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

            // 3. Xóa Session
            HttpContext.Session.Remove("GioHang");

            return RedirectToAction("OrderSuccess");
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}