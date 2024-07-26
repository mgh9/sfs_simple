using SFS.Application.Abstractions.Repositories;
using SFS.Domain.Models;

namespace SFS.Persistence.Repositories;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(IUnitOfWork unitOfWork) 
        : base(unitOfWork)
    {
    }
}
