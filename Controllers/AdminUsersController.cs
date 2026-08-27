using foodshop.Data;
using foodshop.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodshop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AdminUsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userRepository.GetAllUsers();

            var result = users.Select(u => new UserListDto
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber
            });

            return Ok(result);
        }

        [HttpPatch("{id}/reset-password")]
        public IActionResult ResetUserPassword(Guid id, AdminPasswordResetDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
            {
                return BadRequest(new { message = "A jelszónak legalább 6 karakternek kell lennie." });
            }

            var success = _userRepository.AdminResetPassword(id, dto.NewPassword);
            if (!success) return NotFound(new { message = "Felhasználó nem található." });

            return Ok(new { message = "Jelszó sikeresen módosítva!" });
        }

        [HttpPatch("{id}/role")]
        public IActionResult UpdateUserRole(Guid id, AdminRoleUpdateDto dto)
        {
            var validRoles = new[] { "Admin", "User" };
            if (!validRoles.Contains(dto.Role))
            {
                return BadRequest(new { message = "Érvénytelen jogosultság. Csak 'Admin' vagy 'User' lehet." });
            }

            var success = _userRepository.UpdateUserRole(id, dto.Role);
            if (!success) return NotFound(new { message = "Felhasználó nem található." });

            return Ok(new { message = $"A felhasználó jogosultsága módosítva lett erre: {dto.Role}" });
        }
    }
}