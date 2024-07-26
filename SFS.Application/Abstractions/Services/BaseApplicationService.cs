using AutoMapper;
using SFS.Application.Abstractions.Repositories;

namespace SFS.Application.Abstractions.Services;

public abstract class BaseApplicationService
{
    protected IUnitOfWork UnitOfWork { get; }
    protected IMapper Mapper { get; }

    public BaseApplicationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        UnitOfWork = unitOfWork;
        Mapper = mapper;
    }
}
