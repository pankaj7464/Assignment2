namespace Promact.PasswordlessAuthentication.Services
{
    public class UserDetails
    {
        public string Email { get; set; }
        public string Name { get; set; }

        public string[] Roles { get; set; }

        public string UserName { get; set; }
        public  Guid Id { get; set; }
    }
}