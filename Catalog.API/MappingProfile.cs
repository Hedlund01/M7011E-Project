
using AutoMapper;
using Catalog.API.Models;

namespace Catalog.API;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<CreateUpdateProductModel, Product>();
        CreateMap<CreateUpdateCategoryModel, Category>();
        CreateMap<CreateUpdateSpecificationModel, Specifications>();
        CreateMap<CreateUpdateTagModel, Tags>();
    }
}