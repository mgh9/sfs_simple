using SFS.Domain.Models;

namespace SFS.Application.Abstractions.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByTitleAsync(string title, CancellationToken cancellationToken);
}
