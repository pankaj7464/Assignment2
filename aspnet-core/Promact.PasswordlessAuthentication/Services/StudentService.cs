using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;
using Promact.PasswordlessAuthentication.Entities;
using Promact.PasswordlessAuthentication.Services.Dtos;
using static Volo.Abp.UI.Navigation.DefaultMenuNames.Application;
using Promact.PasswordlessAuthentication.Services.Interfaces;

namespace Promact.PasswordlessAuthentication.Services
{

    public class StudentService : CrudAppService<Student, StudentDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateStudentDto, CreateUpdateStudentDto>,
     IStudentAppService
    {
        private readonly IRepository<Student, Guid> _studentRepository;

        public StudentService(IRepository<Student, Guid> repository) : base(repository)
        {
            _studentRepository = repository;
        }

        public override async Task<StudentDto> CreateAsync(CreateUpdateStudentDto input)
        {
            if (await ValidateEmail(input.Email))
            {
                throw new BusinessException($"Email '{input.Email}' already exists.");
            }
            return await base.CreateAsync(input);
        }

        public override async Task<StudentDto> UpdateAsync(Guid id, CreateUpdateStudentDto input)
        {
           if (await ValidateEmail(input.Email))
            {
                throw new BusinessException($"Email '{input.Email}' already exists.");
            }

            return await base.UpdateAsync(id, input);
        }

        private async Task<bool> ValidateEmail(string email)
        {
            var existingStudent =await _studentRepository.FirstOrDefaultAsync(s => s.Email == email);
            if (existingStudent != null)
            {
                return true;
            }
            return false;
        }
    }
}
