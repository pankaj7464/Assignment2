using Microsoft.AspNetCore.Authentication;
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
        public string PasswordlessLoginUrl { get; set; }
        public PasswordlessController(IdentityUserManager userManager, IIdentityUserRepository userRepository)
        {
            UserManager = userManager;
            _userRepository = userRepository;
            UserClaimsPrincipalFactory<Volo.Abp.Identity.IdentityUser> claimsPrincipalFactory;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync(string email)
        {
            // Check if the user with the provided email already exists
            var existingUser = await UserManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return BadRequest("User with this email already exists.");
            }

            // Create a new user
            var newUser = new Volo.Abp.Identity.IdentityUser(Guid.NewGuid(), email, email);
            var result = await UserManager.CreateAsync(newUser);
            if (!result.Succeeded)
            {
                return BadRequest("Failed to create user.");
            }

            // Generate magic link token for the new user
            var token = await UserManager.GenerateUserTokenAsync(newUser, "PasswordlessLoginProvider", "passwordless-auth");

            // Send the magic link to the user's email or display it on the registration page
            PasswordlessLoginUrl = Url.Action("Login", "Passwordless", new { token = token, userId = newUser.Id.ToString() }, Request.Scheme);
            // For demonstration purposes, we'll return the token
            return Ok(PasswordlessLoginUrl);
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

            return Ok("Successfully logged in"); // HTTP 200
        }


        [HttpGet("getall-login-user")]
        public async Task<IActionResult> GetLoggedInUsersAsync()
        {
            var loggedInUsers = new List<LoggedInUserInfo>();

            // Get all user claims
            var allUserClaims = await UserManager.Users
                .SelectMany(u => u.Claims)
                .ToListAsync();

            // Filter out the claims related to logged-in users
            var loggedInUserClaims = allUserClaims
                .Where(c => c.ClaimType == ClaimTypes.AuthenticationMethod && c.ClaimValue == "PasswordlessLoginProvider")
                .ToList();

            // Extract user IDs of logged-in users
            var loggedInUserIds = loggedInUserClaims
                .Select(c => c.UserId)
                .ToList();

            // Retrieve user details for logged-in users
            foreach (var userId in loggedInUserIds)
            {
                var user = await UserManager.FindByIdAsync(userId.ToString());

                if (user != null)
                {
                    var principal = await _claimsPrincipalFactory.CreateAsync(user);

                    // Extract user details from claims
                    var userInfo = new LoggedInUserInfo
                    {
                        UserId = user.Id,
                        UserName = principal.FindFirstValue(AbpClaimTypes.UserName),
                        Email = principal.FindFirstValue(AbpClaimTypes.Email)
                        // Add more properties as needed
                    };

                    loggedInUsers.Add(userInfo);
                }
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

        [HttpGet("get-login-link")]
        public async Task<string> GetLoginLink(string email)
        {
            var user = await _userManager.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new UserFriendlyException("User not found.");
            }

            var token = await _userTokenProvider.GetUserTokenAsync(user, "LoginToken");
            var loginLink = $"https://yourapp.com/account/login?token={token}";

            // Send the loginLink to the user's email
            // ...

            return loginLink;
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }

    public class LoggedInUserInfo
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
       
    }
}
