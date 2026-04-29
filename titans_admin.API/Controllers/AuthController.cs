using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using titans_admin.Models.Entities;
using titans_admin.Models.Enums;
using titans_admin.Repositories.Interfaces;
using titans_admin.Utilities;

namespace titans_admin.API.Controllers;

/// <summary>
/// Authentication API Controller - Handles login, registration, and JWT token generation
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthController> logger) : ControllerBase
{
    /// <summary>
    /// Login endpoint - Authenticates user and returns JWT token
    /// </summary>
    /// <param name="request">Login credentials (email and password)</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Email and password are required" });

            var user = await userRepository.GetByEmailAsync(request.Email);

            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            if (user.Status != UserStatus.Active)
            {
                var statusMessage = user.Status switch
                {
                    UserStatus.Inactive => "Your account is inactive. Please contact administrator.",
                    UserStatus.Suspended => "Your account has been suspended. Please contact administrator.",
                    UserStatus.PendingApproval => "Your account is pending approval. Please wait for administrator approval.",
                    _ => "Account is not active. Please contact administrator."
                };
                return Unauthorized(new { message = statusMessage });
            }

            if (!PasswordHashGenerator.VerifyHash(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password" });

            // Update last login time
            user.LastLoginAt = DateTime.UtcNow;
            await userRepository.UpdateAsync(user);

            // Generate JWT token
            var token = GenerateJwtToken(user);

            logger.LogInformation("User {Email} logged in successfully", user.Email);

            return Ok(new LoginResponse(
                Token: token,
                User: new UserResponse(
                    UserId: user.UserId,
                    Username: user.Username,
                    Email: user.Email,
                    FirstName: user.FirstName,
                    LastName: user.LastName,
                    Role: user.Role.ToString(),
                    FullName: $"{user.FirstName} {user.LastName}".Trim()
                )
            ));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login for user {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during login. Please try again." });
        }
    }

    /// <summary>
    /// Register endpoint - Creates a new user account
    /// </summary>
    /// <param name="request">Registration data (email, password, name, role, etc.)</param>
    /// <returns>Success message</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email) || !IsValidEmail(request.Email))
                return BadRequest(new { message = "Please enter a valid email address" });

            if (string.IsNullOrEmpty(request.FirstName) || request.FirstName.Length < 2)
                return BadRequest(new { message = "First name must be at least 2 characters" });

            if (string.IsNullOrEmpty(request.LastName) || request.LastName.Length < 2)
                return BadRequest(new { message = "Last name must be at least 2 characters" });

            if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 6)
                return BadRequest(new { message = "Password must be at least 6 characters" });

            if (request.Password != request.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match" });

            if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
                return BadRequest(new { message = "Please select a valid user type" });

            if (role == UserRole.Admin)
                return BadRequest(new { message = "Admin accounts cannot be created through registration. Please contact an administrator." });

            if (await userRepository.EmailExistsAsync(request.Email))
                return BadRequest(new { message = "This email is already registered" });

            var username = await GenerateUniqueUsernameAsync(request.Email);

            var user = new User
            {
                Username = username,
                Email = request.Email.ToLower().Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PasswordHash = PasswordHashGenerator.GenerateHash(request.Password),
                Role = role,
                Status = UserStatus.Active,
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim(),
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await userRepository.AddAsync(user);

            logger.LogInformation("New user {Email} registered successfully", user.Email);

            return Ok(new { message = "Account created successfully! You can now log in." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during registration for email {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during registration. Please try again." });
        }
    }

    /// <summary>
    /// Get available user types/roles
    /// </summary>
    /// <returns>List of available user roles</returns>
    [HttpGet("user-types")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetUserTypes()
    {
        return Ok(new[]
        {
            new { value = "Business", label = "Business / Trader" },
            new { value = "Officer", label = "Trade Officer" },
            new { value = "Manager", label = "Program Manager" },
            new { value = "Compliance", label = "Compliance Officer" },
            new { value = "Auditor", label = "Government Auditor" }
        });
    }

    /// <summary>
    /// Verify if user is authenticated
    /// </summary>
    /// <returns>Authentication status</returns>
    [HttpGet("verify")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<object> VerifyToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        return Ok(new
        {
            authenticated = true,
            userId = userId,
            email = email
        });
    }

    /// <summary>
    /// Generate JWT token for authenticated user
    /// </summary>
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["Key"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "1440");

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Validate email address format
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Generate unique username from email
    /// </summary>
    private async Task<string> GenerateUniqueUsernameAsync(string email)
    {
        var baseUsername = System.Text.RegularExpressions.Regex.Replace(
            email.Split('@')[0].ToLower(), "[^a-z0-9_]", "");

        if (string.IsNullOrEmpty(baseUsername))
            baseUsername = "user";

        var username = baseUsername;
        var counter = 1;

        while (await userRepository.UsernameExistsAsync(username))
        {
            username = $"{baseUsername}{counter}";
            counter++;
        }

        return username;
    }
}

/// <summary>
/// Login request model
/// </summary>
public record LoginRequest(string Email = "", string Password = "");

/// <summary>
/// Login response model
/// </summary>
public record LoginResponse(string Token, UserResponse User);

/// <summary>
/// User response model
/// </summary>
public record UserResponse(
    int UserId,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    string FullName
);

/// <summary>
/// Registration request model
/// </summary>
public record RegisterRequest(
    string Email = "",
    string FirstName = "",
    string LastName = "",
    string Password = "",
    string ConfirmPassword = "",
    string Role = "Business",
    string? PhoneNumber = null,
    string? Address = null
);
