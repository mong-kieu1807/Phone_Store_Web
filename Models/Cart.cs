using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneStore.Models
{
	public class Cart
	{
		[Key]
		public int cart_id { get; set; }                         
		public int user_id { get; set; }
		public int product_id { get; set; }
		public int quantity { get; set; }
        public DateTime createdAt { get; set; } 

    }
}