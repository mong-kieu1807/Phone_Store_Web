using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneStore.Models
{
	public class Review
	{
		[Key]
		public int review_id { get; set; }                         
		public int product_id { get; set; }                         
		public int user_id { get; set; }   
		public int rating { get; set; }                         
		public string comment { get; set; } = " ";
		public DateTime created_at { get; set; }                    
		public DateTime updated_at { get; set; }                    
		public bool status { get; set; }
    }
}