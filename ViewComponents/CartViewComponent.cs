using Microsoft.AspNetCore.Mvc;
using PhoneStore.Models;
using System.Text.Json;

namespace PhoneStore.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // 1. Lấy giỏ hàng từ Session
            var session = HttpContext.Session;
            string json = session.GetString("GioHang");
            var gioHang = new List<CartItem>();

            // Nếu có dữ liệu thì chuyển từ JSON sang List
            if (!string.IsNullOrEmpty(json))
            {
                gioHang = JsonSerializer.Deserialize<List<CartItem>>(json);
            }

            // 2. Tính toán số liệu để hiển thị ra Mini Cart
            ViewBag.TotalQuantity = gioHang.Sum(x => x.Quantity);
            ViewBag.TotalPrice = gioHang.Sum(x => x.Total);

            // 3. Trả về View cho Component
            return View(gioHang);
        }
    }
}