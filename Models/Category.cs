using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneStore.Models
{
	public class Category
	{
		[Key]
		public int category_id { get; set; }                         
		public string category_name { get; set; } = " ";
		public string description { get; set; } = " ";
		public bool status { get; set; }
        public DateTime created_at { get; set; } 
        public DateTime updated_at { get; set; } 

    }
}