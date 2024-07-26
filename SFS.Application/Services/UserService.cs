using AutoMapper;
using SFS.Application.Abstractions.Repositories;
using SFS.Application.Abstractions.Services;
using SFS.Domain.Dtos;

namespace SFS.Application.Services;

internal class UserService : BaseApplicationService, IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper) 
        : base(unitOfWork, mapper)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> GetAsync(CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAsync(cancellationToken: cancellationToken);
        return Mapper.Map<List<UserDto>>(users);
    }
}
