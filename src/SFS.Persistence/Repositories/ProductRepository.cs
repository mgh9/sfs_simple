using Microsoft.EntityFrameworkCore;
using SFS.Application.Abstractions.Repositories;
using SFS.Domain.Models;

namespace SFS.Persistence.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<Product?> GetByTitleAsync(string title, CancellationToken cancellationToken)
    {
        return await DbSet.SingleOrDefaultAsync(x => x.Title == title, cancellationToken: cancellationToken);
    }
}
