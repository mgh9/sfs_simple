using Microsoft.EntityFrameworkCore;

namespace SFS.Application.Abstractions.Repositories;

public interface IUnitOfWork
{
    DbContext Context { get; }

    //IProductRepository ProductRepository { get; }
    //IUserRepository UserRepository { get; }
    //IOrderRepository OrderRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
