using Microsoft.EntityFrameworkCore;
using SFS.Application.Abstractions.Repositories;

namespace SFS.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    //public IProductRepository ProductRepository { get; }
    //public IUserRepository UserRepository { get; }
    //public IOrderRepository OrderRepository { get; }

    public DbContext Context => _context;

    public UnitOfWork(AppDbContext context
                        //, IProductRepository productRepository
                        //, IUserRepository userRepository
                        //, IOrderRepository orderRepository
                        )
    {
        _context = context;
        //ProductRepository = productRepository;
        //UserRepository = userRepository;
        //OrderRepository = orderRepository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
