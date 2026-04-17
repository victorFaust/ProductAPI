using AutoMapper;
using Products.Application.Common.DTOs;
using Products.Domain.Entities;

namespace Products.Application.Common.MappingProfiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
    }
}
