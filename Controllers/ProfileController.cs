using foodshop.Data;
using foodshop.DTOs;
using foodshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace foodshop.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    IUserRepository _userRepository;

    public ProfileController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    private Guid GetUserId()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid.TryParse(userIdString, out Guid userId);
        return userId;
    }

    [HttpGet]
    public IActionResult GetMyProfile()
    {
        var user = _userRepository.GetUserById(GetUserId());
        if (user == null) return NotFound("Felhasználó nem található.");

        var profileDto = new UserProfileDto
        {
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            PostalCode = user.PostalCode,
            City = user.City,
            Address = user.Address
        };

        return Ok(profileDto);
    }

    [HttpPut]
    public IActionResult UpdateMyProfile(UserProfileUpdateDto dto)
    {
        var success = _userRepository.UpdateUserProfile(GetUserId(), dto);
        if (!success) return BadRequest("Hiba történt a profil frissítésekor.");

        return Ok(new { message = "Profil sikeresen frissítve!" });
    }

}