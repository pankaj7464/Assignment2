using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

namespace Promact.PasswordlessAuthentication.Controllers
{
    public class PasswordlessController : AbpController
    {

        protected IdentityUserManager UserManager { get; }
        private readonly IIdentityUserRepository _userRepository;
        private readonly UserClaimsPrincipalFactory<Volo.Abp.Identity.IdentityUser> _claimsPrincipalFactory;
        private readonly ICurrentUser _currentUser;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public string PasswordlessLoginUrl { get; set; }
        public PasswordlessController(
            IdentityUserManager userManager,
            IHttpContextAccessor httpContextAccessor,
            IIdentityUserRepository userRepository,
            ICurrentUser currentUser)
        {
            _httpContextAccessor = httpContextAccessor;
            UserManager = userManager;
            _userRepository = userRepository;
            _currentUser = currentUser;

        }

        [HttpPost("register")]
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
                // You can implement your email sending logic here
                var passwordlessLoginUrl = Url.Action("Login", "Passwordless", new { token = token, userId = newUser.Id.ToString() }, Request.Scheme);

                return Ok(new {message = "User created successfully. Magic link sent to email." ,link = passwordlessLoginUrl });
            }
            else // If the user exists, send a magic link
            {
                // Generate magic link token for the existing user
                var token = await UserManager.GenerateUserTokenAsync(existingUser, "PasswordlessLoginProvider", "passwordless-auth");

                // Send the magic link to the user's email
                // You can implement your email sending logic here
                var passwordlessLoginUrl = Url.Action("Login", "Passwordless", new { token = token, userId = existingUser.Id.ToString() }, Request.Scheme);

                return Ok(new { message = "You are exising user. Magic link sent to email.", link = passwordlessLoginUrl });
            }
        }



        [HttpGet("login")]
        public async Task<IActionResult> Login(string token, string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);

            var isValid = await UserManager.VerifyUserTokenAsync(user, "PasswordlessLoginProvider", "passwordless-auth", token);
            if (!isValid)
            {
                return Unauthorized("Invalid token");
            }

            await UserManager.UpdateSecurityStampAsync(user);

            var roles = await UserManager.GetRolesAsync(user);

            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(CreateClaims(user, roles), IdentityConstants.ApplicationScheme)
            );

            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);
            _httpContextAccessor.HttpContext.Session.SetString("UserId", userId);
            return Ok("Successfully logged in"); // HTTP 200
        }


        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            await UserManager.UpdateSecurityStampAsync(user);

            await HttpContext.SignOutAsync();
            _httpContextAccessor.HttpContext.Session.Remove("UserId");
            return Ok("User Logged out successfully");
        }

        [HttpGet("getall-loggedin-user")]
        public IActionResult GetLoggedInUsers()
        {
            var loggedInUsers = new List<string>();

            // Retrieve all active session keys
            var sessionKeys = _httpContextAccessor.HttpContext.Session.Keys;

            // Iterate through session keys to find logged-in users
            foreach (var sessionKey in sessionKeys)
            {
                var sessionValue = _httpContextAccessor.HttpContext.Session.GetString(sessionKey);

                // Assuming your session key represents user ID
                loggedInUsers.Add(sessionValue);
            }

            return Ok(loggedInUsers);
        }

        private static IEnumerable<Claim> CreateClaims(IUser user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim("sub", user.Id.ToString()),
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

     
    }

    
}
