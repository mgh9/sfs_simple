using SFS.Domain.Abstractions;

namespace SFS.Domain.Models;

public class Order : BaseEntity
{
    public Product Product { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public User Buyer { get; set; }
}
