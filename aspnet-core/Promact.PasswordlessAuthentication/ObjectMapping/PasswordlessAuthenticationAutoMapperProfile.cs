using AutoMapper;
using Promact.PasswordlessAuthentication.Entities;
using Promact.PasswordlessAuthentication.Services.Dtos;

namespace Promact.PasswordlessAuthentication.ObjectMapping;

public class PasswordlessAuthenticationAutoMapperProfile : Profile
{
    public PasswordlessAuthenticationAutoMapperProfile()
    {
        /* Create your AutoMapper object mappings here */
        CreateMap<Student, StudentDto>();
        CreateMap<CreateUpdateStudentDto, Student>();
    }
}
