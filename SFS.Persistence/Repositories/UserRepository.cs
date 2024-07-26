using SFS.Application.Abstractions.Repositories;
using SFS.Domain.Models;

namespace SFS.Persistence.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(IUnitOfWork unitOfWork) 
        : base(unitOfWork)
    {
    }
}
