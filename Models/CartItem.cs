using System;

namespace PhoneStore.Models 
{
	// Class này là ViewModel: Kết hợp giữa bảng Carts và Products để hiển thị
	public class CartItem
	{
		public int ProductId { get; set; }    
		public string ProductName { get; set; } 
		public string Image { get; set; }       
		public decimal Price { get; set; }      
		public int Quantity { get; set; }     

		// Thuộc tính tính thành tiền 
		public decimal Total => Price * Quantity;
	}
}