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

public class AuthController : ControllerBase
{
    IAuthRepository _authRepository;
    private readonly IConfiguration _config;

    public AuthController(IAuthRepository authRepository, IConfiguration config)
    {
        _authRepository = authRepository;
        _config = config;

    }


    [HttpPost("Register")]
    public IActionResult Register(UserRegisterDto userRegisterDto)
    {
        try
        {
            var user = _authRepository.Register(userRegisterDto);
            return Ok(new { message = "Sikeres regisztráció!" });
        }

        catch (Exception ex)
        {

            return BadRequest(new { message = "Hiba a regisztráció során.", error = ex.Message });
        }
    }

    [HttpPost("login")]
    public IActionResult Login(UserLoginDto userLoginDto)
    {

        var user = _authRepository.Login(userLoginDto);

        if (user == null)
        {
            return Unauthorized(new { message = "Hibás email vagy jelszó!" });
        }

        // 2. Rövid élettartamú JWT Token generálása (pl. 15 perc)
        string jwtToken = GenerateJwtToken(user);

        // 3. Hosszú élettartamú Refresh Token generálása (pl. 7 nap)
        string refreshToken = GenerateRefreshToken();

        // 4. Refresh token mentése a User táblába
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        _authRepository.SaveChanges();

        // 5. Biztonságos HttpOnly Sütik (Cookies) beállítása a válaszban
        SetCookies(jwtToken, refreshToken);

        return Ok(new { message = "Sikeres bejelentkezés!" });
    }


    [HttpGet("me")]
    [Authorize]
    public IActionResult GetMe()
    {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            Message = "Sikeresen azonosítva!",
            Id = userId,
            Email = userEmail,
            Role = userRole
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {

        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;


        if (!string.IsNullOrEmpty(userEmail))
        {
            _authRepository.RevokeRefreshToken(userEmail);
        }

        Response.Cookies.Delete("jwt");


        Response.Cookies.Delete("refreshToken");


        return Ok(new { message = "Sikeres kijelentkezés, a munkamenet véget ért!" });
    }


    private string GenerateJwtToken(User user)
    {
        // A JWT-be kódoljuk a legfontosabb adatokat (Claims)
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

        // A titkos kulcs lekérése az appsettings.json-ből
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            Issuer = _config.GetSection("Jwt:Issuer").Value,
            Audience = _config.GetSection("Jwt:Audience").Value,
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private void SetCookies(string jwtToken, string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // XSS védelem: A kliensoldali JavaScript nem férhet hozzá
            Secure = true,   // Csak HTTPS kapcsolaton keresztül küldi át (localhoston is működik általában)
            SameSite = SameSiteMode.Strict, // CSRF védelem
            Expires = DateTime.UtcNow.AddDays(7)
        };

        // Hozzáadjuk a sütiket a válaszhoz (Response)
        Response.Cookies.Append("jwt", jwtToken, cookieOptions);
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
