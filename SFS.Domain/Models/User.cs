using SFS.Domain.Abstractions;

namespace SFS.Domain.Models;

public class User : BaseEntity
{
    public required string Name { get; set; }
    public List<Order> Orders { get; set; } = [];
}
