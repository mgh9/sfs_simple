using SFS.Domain.Abstractions;

namespace SFS.Application.Abstractions.Repositories;

public interface IRepository<TEntity>
    where TEntity : BaseEntity
{
    Task<List<TEntity>> GetAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetAsync(int Id, CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}
