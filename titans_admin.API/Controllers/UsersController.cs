using Microsoft.AspNetCore.Mvc;
using titans_admin.Models.ViewModels;
using titans_admin.Services.Interfaces;

namespace titans_admin.API.Controllers;

/// <summary>
/// User management endpoints for CRUD operations and filtering
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController(IAdminService adminService, ILogger<UsersController> logger) : ControllerBase
{
    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<UserListItemViewModel>>> GetAllUsers()
    {
        try
        {
            var users = await adminService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching all users");
            return StatusCode(500, new { message = "An error occurred while fetching users" });
        }
    }

    /// <summary>
    /// Get a user by ID
    /// </summary>
    /// <param name="id">The user ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserEditViewModel>> GetUserById(int id)
    {
        try
        {
            var user = await adminService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching user {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while fetching the user" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateUser([FromBody] CreateUserViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, errorMessage, userId) = await adminService.CreateUserAsync(model);

            if (!success)
                return BadRequest(new { message = errorMessage ?? "Failed to create user" });

            return CreatedAtAction(nameof(GetUserById), new { id = userId }, new { userId });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user {Email}", model.Email);
            return StatusCode(500, new { message = "An error occurred while creating the user" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserEditViewModel model)
    {
        try
        {
            if (id != model.UserId)
                return BadRequest(new { message = "User ID mismatch" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await adminService.UpdateUserAsync(model);

            if (!success)
                return BadRequest(new { message = "Failed to update user" });

            return Ok(new { message = "User updated successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the user" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var success = await adminService.DeleteUserAsync(id);

            if (!success)
                return BadRequest(new { message = "Failed to delete user" });

            return Ok(new { message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the user" });
        }
    }

    [HttpGet("by-role/{role}")]
    public async Task<ActionResult<List<UserListItemViewModel>>> GetUsersByRole(string role)
    {
        try
        {
            if (!Enum.TryParse<titans_admin.Models.Enums.UserRole>(role, ignoreCase: true, out var userRole))
                return BadRequest(new { message = "Invalid user role" });

            var users = await adminService.GetUsersByRoleAsync(userRole);
            return Ok(users);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching users by role {Role}", role);
            return StatusCode(500, new { message = "An error occurred while fetching users" });
        }
    }

    [HttpGet("by-status/{status}")]
    public async Task<ActionResult<List<UserListItemViewModel>>> GetUsersByStatus(string status)
    {
        try
        {
            if (!Enum.TryParse<titans_admin.Models.Enums.UserStatus>(status, ignoreCase: true, out var userStatus))
                return BadRequest(new { message = "Invalid user status" });

            var users = await adminService.GetUsersByStatusAsync(userStatus);
            return Ok(users);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching users by status {Status}", status);
            return StatusCode(500, new { message = "An error occurred while fetching users" });
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] UpdateUserStatusRequest request)
    {
        try
        {
            if (!Enum.TryParse<titans_admin.Models.Enums.UserStatus>(request.Status, ignoreCase: true, out var userStatus))
                return BadRequest(new { message = "Invalid user status" });

            var success = await adminService.UpdateUserStatusAsync(id, userStatus, request.ModifiedByUserId);

            if (!success)
                return BadRequest(new { message = "Failed to update user status" });

            return Ok(new { message = "User status updated successfully" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user status {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while updating user status" });
        }
    }
}

public record UpdateUserStatusRequest(string Status, int ModifiedByUserId);
