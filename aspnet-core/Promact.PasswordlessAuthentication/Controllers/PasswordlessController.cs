using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Promact.PasswordlessAuthentication.Services;
using Promact.PasswordlessAuthentication.Services.Emailing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
namespace Promact.PasswordlessAuthentication.Controllers
{

    public class PasswordlessController : AbpController
    {

        protected IdentityUserManager UserManager;
        private readonly ICurrentUser _currentUser;
        private readonly IHubContext<UserHub> _hubContext;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IRepository<Volo.Abp.Identity.IdentityUser,Guid> _userRepository;
        public string PasswordlessLoginUrl { get; set; }
        public PasswordlessController(
            IdentityUserManager userManager,
            IHubContext<UserHub> hubContext,
            IRepository<Volo.Abp.Identity.IdentityUser,Guid> userRepositoy,
            IConfiguration config,
            IEmailService emailService,
            ICurrentUser currentUser)
        {
            _hubContext = hubContext;
             UserManager = userManager;
            _currentUser = currentUser;
            _emailService = emailService;
            _config = config;
            _userRepository = userRepositoy;

        }

        [HttpPost("api/app/register")]
        public async Task<IActionResult> RegisterUserAsync(string email)
        {
           // Check if the user with the provided email exists
            var existingUser = await UserManager.FindByEmailAsync(email);
          
            // Split the email address at the "@" symbol
            string[] firstpart = email.Split('@');

            // Take the part before "@" as the username
            string username = firstpart[0];
            // If the user doesn't exist, create a new account
            if (existingUser == null)
            {

                var newUser = new Volo.Abp.Identity.IdentityUser(Guid.NewGuid(), username, email);
                var result = await UserManager.CreateAsync(newUser);
                if (!result.Succeeded)
                {
                    return BadRequest("Failed to create user.");
                }

                // Generate magic link token for the new user
                var token = await UserManager.GenerateUserTokenAsync(newUser, "PasswordlessLoginProvider", "passwordless-auth");

                // Send the magic link to the user's email
                var passwordlessLoginUrl = $"{_config["App:ClientUrl"]}/authenticate?token={token}&userId={newUser.Id}";
                
                // Send the magic link to email
                await SendEmail(username, email, passwordlessLoginUrl);
                return Ok(new {message = "User created successfully. Magic link sent to email." ,link = passwordlessLoginUrl });
            }
            else // If the user exists, send a magic link
            {
                // Generate magic link token for the existing user
                var token = await UserManager.GenerateUserTokenAsync(existingUser, "PasswordlessLoginProvider", "passwordless-auth");

                // Send the magic link to the user's email
                var passwordlessLoginUrl = $"{_config["App:ClientUrl"]}/authenticate?token={token}&userId={existingUser.Id}";
               await SendEmail(username, email,passwordlessLoginUrl);
                return Ok(new { message = "You are exising user. Magic link sent to email.", link = passwordlessLoginUrl });
            }
        }



        [HttpGet("api/app/login")]
        public async Task<IActionResult> Login(string token, string userId)
        {
            var u = _currentUser;
            var user = await UserManager.FindByIdAsync(userId);
            var isValid = await UserManager.VerifyUserTokenAsync(user, "PasswordlessLoginProvider", "passwordless-auth", token);
            if (!isValid)
            {
                return Unauthorized("Invalid token");
            }
            await UserManager.UpdateSecurityStampAsync(user);

            var roles = await UserManager.GetRolesAsync(user);
            var claims = CreateClaims(user, roles);

            //await _signinManager.SignInAsync(user, true);

            var jwt_token =await GenerateJwtTokenAsync(user, claims);
            var userDetail = new UserDetails
            {
                Name = user.UserName,
                Email = user.Email,
                UserName = user.UserName,
                Id  = user.Id,
                Roles = roles.ToArray(),
             
            };

          
            var users = await _userRepository.GetQueryableAsync();
            
            await _hubContext.Clients.All.SendAsync("ReceiveUserDetailOnConnect",userDetail);
            UserHub.totalUser = users.Count();

            return Ok(new {message = "\"Successfully logged in\"" ,token = jwt_token,user = user}); // HTTP 200
        }

        [HttpGet("api/app/logout")]
        [Authorize]
        public async Task<IActionResult> Logout(string userId)
        {
            var c = _currentUser;
            var user = await UserManager.FindByIdAsync(userId);
            await UserManager.UpdateSecurityStampAsync(user);

            //await _signinManager.SignOutAsync();
                c = _currentUser;
            var userDetail = new UserDetails
            {
                Name = user.UserName,
                Email = user.Email,
                UserName = user.UserName,
                Id = user.Id

            };
            // Find and remove the user with the specified email address
            var users = await _userRepository.GetQueryableAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveUserDetailOnConnect", userDetail);
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", 
                new {activeUser= UserHub.activeUser,totalUser = users.Count()});
           

            return Ok("User Logged out successfully");
        }

        [HttpGet("api/app/alluser")]
        public async Task<IActionResult> GetAllUser()
        {

            var user = await _userRepository.GetQueryableAsync();
            var t = user.Count();
            await _hubContext.Clients.All.SendAsync("ReceiveUserDetailOnDisConnect", new
            {
              
                allUser = UserHub.connectedUsers,
                totalUser = user.Count()
            });

            return Ok("User Fetch successfully");
        }

        private static IEnumerable<Claim> CreateClaims(IUser user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(AbpClaimTypes.UserId, user.Id.ToString()),
                new Claim(AbpClaimTypes.Email, user.Email),
                new Claim(AbpClaimTypes.UserName, user.UserName),
                new Claim(AbpClaimTypes.EmailVerified, user.EmailConfirmed.ToString().ToLower()),
            };

            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                claims.Add(new Claim(AbpClaimTypes.PhoneNumber, user.PhoneNumber));
            }

            foreach (var role in roles)
            {
                claims.Add(new Claim(AbpClaimTypes.Role, role));
            }

            return claims;
        }

          private async Task SendEmail(string username ,string email,string link)
        {
            // Prepare email DTO
            var emailDto = new EmailDto
            {
                To = email,
                Subject = "Your Magic Login Link",
                Body =Template.GetEmailTemplate(username,link)
            };

            // Send the magic link email
             _emailService.SendEmail(emailDto);
        }

        private async Task<string> GenerateJwtTokenAsync(IUser user, IEnumerable<Claim> claims)
        {
            
            // Create symmetric security key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:key"]));

            // Create signing credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1), // Token expiry time
                SigningCredentials = creds
            };

            // Create token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Generate token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return token as string
            return tokenHandler.WriteToken(token);
        }




    }

    
}
