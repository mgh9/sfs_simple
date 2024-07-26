using SFS.Domain.Abstractions;

namespace SFS.Domain.Models;

public class Order : BaseEntity
{
    public User Buyer { get; set; }

    public Product Product { get; set; }
    public decimal Price { get; set; }
    public double Discount { get; set; }
    
    public DateTime CreationDate { get; set; } = DateTime.Now;
}
