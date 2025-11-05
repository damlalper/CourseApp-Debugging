using AutoMapper;
using CourseApp.EntityLayer.Dto.CourseDto;
using CourseApp.EntityLayer.Entity;

namespace CourseApp.ServiceLayer.Mapping;

public class CourseMapping : Profile
{
    public CourseMapping()
    {
        CreateMap<Course, GetAllCourseDto>().ReverseMap();
        CreateMap<Course, GetByIdCourseDto>().ReverseMap();
        CreateMap<CreateCourseDto, Course>().ReverseMap();
        CreateMap<UpdateCourseDto, Course>().ReverseMap();
        CreateMap<Course, GetAllCourseDetailDto>().ReverseMap();
    }
}
