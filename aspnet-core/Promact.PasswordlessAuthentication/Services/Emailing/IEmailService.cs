
namespace Promact.PasswordlessAuthentication.Services.Emailing
{
    public interface IEmailService
    {
        void SendEmail(EmailDto request);
   
    }
}
