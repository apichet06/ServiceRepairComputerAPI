using AutoMapper;
using ServiceRepairComputer.Models;
using ServiceRepairComputer.Models.Dto;

namespace ServiceRepairComputer
{
    public class MappingConfig
    {
        public static MapperConfiguration registerMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Employee, EmployeeDto>();
                config.CreateMap<EmployeeDto, Employee>();
                config.CreateMap<Computer, ComputerDto>();
                config.CreateMap<ComputerDto, Computer>();
                config.CreateMap<Issue, IssueDto>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
                config.CreateMap<IssueDto, Issue>();
                config.CreateMap<Categories, CategoriesDto>();
                config.CreateMap<CategoriesDto, Categories>();
                config.CreateMap<Comment,CommentDto>();
                config.CreateMap<CommentDto, Comment>();
                config.CreateMap<Division, DivisionDto>();
                config.CreateMap<DivisionDto, Division>();
                config .CreateMap<Position,PositionDto>();
                config.CreateMap<PositionDto, Position>();
            });
            return mappingConfig;


        }



    }
}
