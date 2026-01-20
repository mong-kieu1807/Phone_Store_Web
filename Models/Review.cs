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
		public DateTime? created_at { get; set; }  // Nullable                  
		public DateTime? updated_at { get; set; }  // Nullable                  
		public byte status { get; set; }  // TINYINT trong SQL Server
    }
}