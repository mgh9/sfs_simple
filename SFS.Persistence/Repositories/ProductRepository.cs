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

    public async Task<bool> IsProductTitleAlreadyExistAsync(string title, CancellationToken cancellationToken)
    {
        return await DbSet.CountAsync(x => x.Title == title, cancellationToken: cancellationToken) > 0;
    }
}
