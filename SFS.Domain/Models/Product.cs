using SFS.Domain.Abstractions;

namespace SFS.Domain.Models;

public class Product : BaseEntity
{
    public required string Title { get; set; }
    public int InventoryCount { get; set; }
    public decimal Price { get; set; }
    public double Discount { get; set; }
}