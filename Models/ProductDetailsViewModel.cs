using System;
using System.Collections.Generic;

namespace PhoneStore.Models
{
    // ViewModel để chứa thông tin hiển thị trang chi tiết sản phẩm
    public class ProductDetailsViewModel
    {
        // Thông tin sản phẩm
        public Product Product { get; set; } = null!;
        
        // Danh sách reviews của sản phẩm
        public List<ReviewWithUser> Reviews { get; set; } = new List<ReviewWithUser>();
        
        // Thống kê rating
        public RatingSummary RatingSummary { get; set; } = new RatingSummary();
    }

    // Class chứa thông tin review kèm thông tin user
    public class ReviewWithUser
    {
        public int review_id { get; set; }
        public int product_id { get; set; }
        public int user_id { get; set; }
        public int rating { get; set; }
        public string comment { get; set; } = string.Empty;
        public DateTime? created_at { get; set; }  // Nullable vì có thể NULL trong DB
        
        // Thông tin user
        public string user_name { get; set; } = string.Empty;
        public string user_email { get; set; } = string.Empty;
    }

    // Class chứa thống kê rating
    public class RatingSummary
    {
        public decimal AverageRating { get; set; }  // Rating trung bình
        public int TotalReviews { get; set; }       // Tổng số reviews
        
        // Số lượng reviews theo từng mức sao
        public int FiveStars { get; set; }
        public int FourStars { get; set; }
        public int ThreeStars { get; set; }
        public int TwoStars { get; set; }
        public int OneStar { get; set; }
        
        // Tính phần trăm cho từng mức sao (để hiển thị thanh progress)
        public int FiveStarsPercent => TotalReviews > 0 ? (FiveStars * 100 / TotalReviews) : 0;
        public int FourStarsPercent => TotalReviews > 0 ? (FourStars * 100 / TotalReviews) : 0;
        public int ThreeStarsPercent => TotalReviews > 0 ? (ThreeStars * 100 / TotalReviews) : 0;
        public int TwoStarsPercent => TotalReviews > 0 ? (TwoStars * 100 / TotalReviews) : 0;
        public int OneStarPercent => TotalReviews > 0 ? (OneStar * 100 / TotalReviews) : 0;
    }
}
