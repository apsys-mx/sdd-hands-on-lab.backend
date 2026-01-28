using AutoMapper;
using kudos.backend.domain.daos;
using kudos.backend.domain.interfaces.repositories;
using kudos.backend.webapi.dtos;

namespace kudos.backend.webapi.mappingprofiles;

/// <summary>
/// AutoMapper profile for Book-related mappings.
/// </summary>
public class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
        // DAO to DTO
        CreateMap<BookDao, BookDto>();

        // GetManyAndCount result mapping
        CreateMap<GetManyAndCountResult<BookDao>, GetManyAndCountResultDto<BookDto>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.SortBy, opt => opt.MapFrom(src => src.Sorting.SortBy))
            .ForMember(dest => dest.SortCriteria, opt => opt.MapFrom(src =>
                src.Sorting.Criteria == SortingCriteriaType.Ascending ? "asc" : "desc"));

        // Note: No Request → Command mapping needed because:
        // - The Request is empty
        // - The Command is constructed manually with the query string
    }
}
