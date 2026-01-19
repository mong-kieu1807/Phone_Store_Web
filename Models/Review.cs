using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneStore.Models
{
    [Table("Reviews")]
    public class Review
    {
        [Key]
        public int review_id { get; set; }
        public int product_id { get; set; }
        public int user_id { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? updated_at { get; set; } // Để null được để tránh lỗi ngày tháng

        public byte status { get; set; } //NTBinh 19/01
    }
}