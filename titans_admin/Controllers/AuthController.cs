using Microsoft.AspNetCore.Mvc;
using titans_admin.Repositories.Interfaces;
using titans_admin.Models.Entities;
using titans_admin.Models.Enums;
using titans_admin.Utilities;

namespace titans_admin.Controllers;

[Route("[controller]")]
public class AuthController(IUserRepository userRepository, ILogger<AuthController> logger) : Controller
{
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return Json(new { success = false, message = "Email and password are required" });

            var user = await userRepository.GetByEmailAsync(request.Email);

            if (user == null)
                return Json(new { success = false, message = "Invalid email or password" });

            if (user.Status != UserStatus.Active)
            {
                var statusMessage = user.Status switch
                {
                    UserStatus.Inactive => "Your account is inactive. Please contact administrator.",
                    UserStatus.Suspended => "Your account has been suspended. Please contact administrator.",
                    UserStatus.PendingApproval => "Your account is pending approval. Please wait for administrator approval.",
                    _ => "Account is not active. Please contact administrator."
                };
                return Json(new { success = false, message = statusMessage });
            }

            if (!PasswordHashGenerator.VerifyHash(request.Password, user.PasswordHash))
                return Json(new { success = false, message = "Invalid email or password" });

            user.LastLoginAt = DateTime.UtcNow;
            await userRepository.UpdateAsync(user);

            logger.LogInformation("User {Email} logged in successfully", user.Email);

            return Json(new
            {
                success = true,
                user = new
                {
                    userId = user.UserId,
                    username = user.Username,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    role = user.Role.ToString(),
                    fullName = $"{user.FirstName} {user.LastName}".Trim()
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login for user {Email}", request.Email);
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email) || !IsValidEmail(request.Email))
                return Json(new { success = false, message = "Please enter a valid email address" });

            if (string.IsNullOrEmpty(request.FirstName) || request.FirstName.Length < 2)
                return Json(new { success = false, message = "First name must be at least 2 characters" });

            if (string.IsNullOrEmpty(request.LastName) || request.LastName.Length < 2)
                return Json(new { success = false, message = "Last name must be at least 2 characters" });

            if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 6)
                return Json(new { success = false, message = "Password must be at least 6 characters" });

            if (request.Password != request.ConfirmPassword)
                return Json(new { success = false, message = "Passwords do not match" });

            if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
                return Json(new { success = false, message = "Please select a valid user type" });

            if (role == UserRole.Admin)
                return Json(new { success = false, message = "Admin accounts cannot be created through registration. Please contact an administrator." });

            if (await userRepository.EmailExistsAsync(request.Email))
                return Json(new { success = false, message = "This email is already registered" });

            var username = await GenerateUniqueUsernameAsync(request.Email);

            var user = new User
            {
                Username = username,
                Email = request.Email.ToLower().Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PasswordHash = PasswordHashGenerator.GenerateHash(request.Password),
                Role = role,
                Status = UserStatus.Active,  // Always set to Active - no approval needed
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim(),
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await userRepository.AddAsync(user);

            logger.LogInformation("New user {Email} registered successfully", user.Email);

            return Json(new
            {
                success = true,
                message = "Account created successfully! You can now log in."
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during registration for email {Email}", request.Email);
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }
    }

    [HttpPost("Logout")]
    public IActionResult Logout() => Json(new { success = true });

    [HttpGet("Check")]
    public IActionResult Check() => Json(new { authenticated = false });

    [HttpGet("UserTypes")]
    public IActionResult GetUserTypes() => Json(new[]
    {
        new { value = "Business", label = "Business / Trader" },
        new { value = "Officer", label = "Trade Officer" },
        new { value = "Manager", label = "Program Manager" },
        new { value = "Compliance", label = "Compliance Officer" },
        new { value = "Auditor", label = "Government Auditor" }
    });

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

public record LoginRequest(string Email = "", string Password = "");

public record RegisterRequest(
    string Email = "",
    string FirstName = "",
    string LastName = "",
    string Password = "",
    string ConfirmPassword = "",
    string Role = "Business",
    string? PhoneNumber = null,
    string? Address = null);
