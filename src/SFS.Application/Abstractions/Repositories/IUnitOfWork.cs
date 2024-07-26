using Microsoft.EntityFrameworkCore;

namespace SFS.Application.Abstractions.Repositories;

public interface IUnitOfWork
{
    DbContext Context { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
