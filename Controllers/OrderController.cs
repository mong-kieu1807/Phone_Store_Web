// NTBinh 14/01/2026 - OrderController (Fixed Auth Redirects)
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Models;
using PhoneStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace PhoneStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- HÀM HỖ TRỢ ---
        private List<CartItem> LayGioHang()
        {
            var session = HttpContext.Session;
            string json = session.GetString("GioHang");
            if (string.IsNullOrEmpty(json)) return new List<CartItem>();
            return JsonSerializer.Deserialize<List<CartItem>>(json);
        }

        private void LuuGioHang(List<CartItem> gioHang)
        {
            var session = HttpContext.Session;
            string json = JsonSerializer.Serialize(gioHang);
            session.SetString("GioHang", json);
        }

        // --- 1. THÊM VÀO GIỎ
        public async Task<IActionResult> AddToCart(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(p => p.ProductId == id);

            if (item != null) item.Quantity++;
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
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // --- 2. MUA NGAY 
        public async Task<IActionResult> BuyNow(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(p => p.ProductId == id);

            if (item != null) item.Quantity++;
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

        // --- 3. GIỎ HÀNG 
        public IActionResult Cart(int page = 1)
        {
            var gioHang = LayGioHang();
            ViewBag.TongTien = gioHang.Sum(p => p.Total);

            int pageSize = 4;
            if (page < 1) page = 1;
            int totalItems = gioHang.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedCart = gioHang.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(pagedCart);
        }

        public IActionResult UpdateCart(int id, int quantity)
        {
            var gioHang = LayGioHang();
            var item = gioHang.FirstOrDefault(p => p.ProductId == id);
            if (item != null)
            {
                item.Quantity = quantity;
                if (item.Quantity < 1) item.Quantity = 1;
                LuuGioHang(gioHang);
            }
            return RedirectToAction("Cart");
        }

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

        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("GioHang");
            return RedirectToAction("Cart");
        }

        // --- 4. THANH TOÁN
        public IActionResult Checkout()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var gioHang = LayGioHang();
            if (gioHang.Count == 0) return RedirectToAction("Index", "Home");

            ViewBag.TongTienDonHang = gioHang.Sum(p => p.Total);
            return View(gioHang);
        }

        // --- 5. XỬ LÝ ĐẶT HÀNG 
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string FullName, string Phone, string Address, string Note)
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var gioHang = LayGioHang();
            if (gioHang.Count == 0) return RedirectToAction("Cart");

            var bill = new Bill
            {
                user_id = sessionUserId.Value,
                total_amount = gioHang.Sum(x => x.Total),
                shipping_status = "Chờ xử lý",
                payment_method_id = 1,
                created_at = DateTime.Now,
                status = 1
            };

            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

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

            HttpContext.Session.Remove("GioHang");
            return RedirectToAction("OrderSuccess");
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}