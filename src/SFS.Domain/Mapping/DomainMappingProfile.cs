using AutoMapper;
using SFS.Domain.Dtos;
using SFS.Domain.Models;

namespace SFS.Domain.Mapping;
public class DomainMappingProfile : Profile
{
    public DomainMappingProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<Order, OrderDto>().ReverseMap();
    }
}
