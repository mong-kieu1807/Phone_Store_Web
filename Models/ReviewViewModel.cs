//NTBinh 19/01
using System;

namespace PhoneStore.Models
{
    public class ReviewAdminViewModel
    {
        public int ReviewId { get; set; }
        public string UserFullName { get; set; }
        public string ProductName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte Status { get; set; } 
    }
    public class ReviewViewModel
    {
        public Review Review { get; set; }
        public string UserFullName { get; set; }
    }
} //NTBinh 19/01