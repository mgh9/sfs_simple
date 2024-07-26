using Microsoft.EntityFrameworkCore;
using SFS.Application.Abstractions.Repositories;
using SFS.Domain.Abstractions;

namespace SFS.Persistence.Repositories;

public abstract class BaseRepository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
{
    private readonly IUnitOfWork _unitOfWork;
    protected readonly DbSet<TEntity> DbSet;

    public BaseRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        DbSet = _unitOfWork.Context.Set<TEntity>();
    }

    public async Task<List<TEntity>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<TEntity?> GetAsync(int Id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([Id], cancellationToken: cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }
}
