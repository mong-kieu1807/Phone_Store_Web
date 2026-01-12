using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneStore.Models
{
	public class GoodsReceipts
	{
		[Key]
		public int import_id { get; set; }                         
		public int supplier_id { get; set; }                  
		public int user_id { get; set; }                  
		public decimal total_cost { get; set; } = 0;
		public byte status { get; set; }
        public DateTime? created_at { get; set; }
    }
}