using Volo.Abp.Application.Dtos;

namespace Promact.PasswordlessAuthentication.Services.Dtos
{
    public class StudentDto:IEntityDto<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
       
    }
}
