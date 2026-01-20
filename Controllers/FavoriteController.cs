using Microsoft.AspNetCore.Mvc;
using PhoneStore.Models;
using PhoneStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace PhoneStore.Controllers
{
    // NTNguyen - Controller xử lý chức năng yêu thích sản phẩm
    public class FavoriteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string KHOA_SESSION_YEU_THICH = "Favorites";

        public FavoriteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách sản phẩm yêu thích từ Session
        private List<FavoriteItem> LayDanhSachYeuThich()
        {
            var jsonYeuThich = HttpContext.Session.GetString(KHOA_SESSION_YEU_THICH);
            if (string.IsNullOrEmpty(jsonYeuThich))
            {
                return new List<FavoriteItem>();
            }
            return JsonSerializer.Deserialize<List<FavoriteItem>>(jsonYeuThich) ?? new List<FavoriteItem>();
        }

        // Lưu danh sách yêu thích vào Session
        private void LuuDanhSachYeuThich(List<FavoriteItem> danhSachYeuThich)
        {
            var jsonYeuThich = JsonSerializer.Serialize(danhSachYeuThich);
            HttpContext.Session.SetString(KHOA_SESSION_YEU_THICH, jsonYeuThich);
        }

        // Thêm sản phẩm vào yêu thích
        [HttpPost]
        [Route("Favorite/Add/{productId}")]
        public async Task<IActionResult> Add(int productId)
        {
            // Kiểm tra đã đăng nhập chưa
            var maNguoiDung = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(maNguoiDung))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để sử dụng tính năng này!" });
            }

            // Lấy thông tin sản phẩm
            var sanPham = await _context.Products.FindAsync(productId);
            if (sanPham == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });
            }

            // Lấy danh sách yêu thích hiện tại
            var danhSachYeuThich = LayDanhSachYeuThich();

            // Kiểm tra đã có trong danh sách chưa
            if (danhSachYeuThich.Any(sp => sp.ProductId == productId))
            {
                return Json(new { success = false, message = "Sản phẩm đã có trong danh sách yêu thích!" });
            }

            // Thêm vào danh sách
            var sanPhamYeuThich = new FavoriteItem
            {
                ProductId = sanPham.product_id,
                ProductName = sanPham.product_name,
                Image = sanPham.image.Split(',')[0].Trim(), // Lấy ảnh đầu tiên
                Price = sanPham.price,
                DiscountPercent = sanPham.discount_percent,
                AddedDate = DateTime.Now
            };

            danhSachYeuThich.Add(sanPhamYeuThich);
            LuuDanhSachYeuThich(danhSachYeuThich);

            return Json(new { success = true, message = "Đã thêm vào danh sách yêu thích!", count = danhSachYeuThich.Count });
        }

        // Xóa sản phẩm khỏi yêu thích
        [HttpPost]
        [Route("Favorite/Remove/{productId}")]
        public IActionResult Remove(int productId)
        {
            // Kiểm tra đã đăng nhập chưa
            var maNguoiDung = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(maNguoiDung))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập!" });
            }

            var danhSachYeuThich = LayDanhSachYeuThich();
            var sanPhamCanXoa = danhSachYeuThich.FirstOrDefault(sp => sp.ProductId == productId);

            if (sanPhamCanXoa != null)
            {
                danhSachYeuThich.Remove(sanPhamCanXoa);
                LuuDanhSachYeuThich(danhSachYeuThich);
                return Json(new { success = true, message = "Đã xóa khỏi danh sách yêu thích!", count = danhSachYeuThich.Count });
            }

            return Json(new { success = false, message = "Sản phẩm không có trong danh sách yêu thích!" });
        }

        // Kiểm tra sản phẩm có trong danh sách yêu thích không
        [Route("Favorite/Check/{productId}")]
        [HttpGet]
        public IActionResult Check(int productId)
        {
            var danhSachYeuThich = LayDanhSachYeuThich();
            var daYeuThich = danhSachYeuThich.Any(sp => sp.ProductId == productId);
            return Json(new { isFavorite = daYeuThich });
        }

        // Hiển thị danh sách sản phẩm yêu thích
        public IActionResult Index()
        {
            // Kiểm tra đã đăng nhập chưa
            var maNguoiDung = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(maNguoiDung))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem danh sách yêu thích!";
                return RedirectToAction("Login", "Auth");
            }

            var danhSachYeuThich = LayDanhSachYeuThich();
            return View(danhSachYeuThich);
        }

        // Xóa tất cả sản phẩm yêu thích
        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(KHOA_SESSION_YEU_THICH);
            return Json(new { success = true, message = "Đã xóa tất cả sản phẩm yêu thích!" });
        }

        // Đếm số lượng sản phẩm yêu thích
        [HttpGet]
        public IActionResult Count()
        {
            var danhSachYeuThich = LayDanhSachYeuThich();
            return Json(new { count = danhSachYeuThich.Count });
        }
    }
    // endNTNguyen
}
