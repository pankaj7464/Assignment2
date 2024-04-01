using Microsoft.AspNetCore.Identity;
using Volo.Abp.Identity;

namespace Promact.PasswordlessAuthentication.Entities
{
    public class ApplicationUser:IdentityUser<Guid>
    {
        public bool IsOnline { get; set; }

    }
}
