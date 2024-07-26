namespace SFS.Domain.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int BuyerId { get; set; }
    public decimal Price { get; set; }
    public double Discount { get; set; }
}