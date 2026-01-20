using System;

namespace PhoneStore.Models
{
    // Model đơn giản để lưu sản phẩm yêu thích vào Session
    public class FavoriteItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DiscountPercent { get; set; }
        public DateTime AddedDate { get; set; }  // Ngày thêm vào yêu thích
    }
}
