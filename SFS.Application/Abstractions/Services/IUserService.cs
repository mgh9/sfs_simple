using SFS.Domain.Dtos;

namespace SFS.Application.Abstractions.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAsync(CancellationToken cancellationToken);
}

