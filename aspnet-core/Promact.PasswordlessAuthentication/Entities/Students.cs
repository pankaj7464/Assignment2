using Volo.Abp.Domain.Entities.Auditing;

namespace Promact.PasswordlessAuthentication.Entities
{
    public class Students : AuditedEntity<Guid>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
    }
}
