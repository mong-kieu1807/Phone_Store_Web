namespace PhoneStore.Models;

public class HomeViewModel
{
    public List<Product> NewProducts { get; set; } = new List<Product>();
    public List<Product> TopSelling { get; set; } = new List<Product>();
}