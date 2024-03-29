namespace Promact.PasswordlessAuthentication.Services.Dtos
{

    public class LoggedInUserInfo
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

    }
}
