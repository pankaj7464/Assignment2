using Volo.Abp.Domain.Entities.Auditing;

namespace Promact.PasswordlessAuthentication.Entities
{
    public class Student:AuditedEntity<Guid>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }

      
    }
}
