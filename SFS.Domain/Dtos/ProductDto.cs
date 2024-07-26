namespace SFS.Domain.Dtos;

public class ProductDto
{
    public required string Title { get; set; }
    public int InventoryCount { get; set; }
    public decimal Price { get; set; }
    public double Discount { get; set; }
}