using SFS.Domain.Models;

namespace SFS.Application.Abstractions.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<bool> IsProductTitleAlreadyExistAsync(string title, CancellationToken cancellationToken);
}
