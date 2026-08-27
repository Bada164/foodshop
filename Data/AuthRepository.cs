using System.Security.Cryptography;
using System.Text;
using foodshop.Models;
using foodshop.DTOs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

namespace foodshop.Data
{

    public class AuthRepository : IAuthRepository
    {
        DataContext _entityFramework;
        private readonly IConfiguration _config;

        public AuthRepository(IConfiguration config)
        {
            _entityFramework = new DataContext(config);
            _config = config;
        }

        public User Register(UserRegisterDto userRegisterDto)
        {
            byte[] passwordSalt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(passwordSalt);
            }

            byte[] passwordHash = GetPasswordHash(userRegisterDto.Password, passwordSalt);

            User newUser = new User
            {
                Email = userRegisterDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User"
            };

            _entityFramework.Users.Add(newUser);
            _entityFramework.SaveChanges();

            return newUser;
        }

        public User? Login(UserLoginDto userLoginDto)
        {
            User? user = _entityFramework.Users.FirstOrDefault(u => u.Email == userLoginDto.Email);


            if (user == null)
            {
                return null;
            }


            byte[] computedHash = GetPasswordHash(userLoginDto.Password, user.PasswordSalt);


            if (!computedHash.SequenceEqual(user.PasswordHash))
            {
                return null;
            }


            return user;

        }

        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            );
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }


        public void RevokeRefreshToken(string email)
        {
            var user = _entityFramework.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                _entityFramework.SaveChanges();
            }
        }

    }
}