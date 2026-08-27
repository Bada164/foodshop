using System.Security.Cryptography;
using foodshop.DTOs;
using foodshop.Models;

namespace foodshop.Data
{
    public class UserRepository : IUserRepository
    {
        DataContext _entityFramework;

        private readonly IConfiguration _config;

        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContext(config);
            _config = config;
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public IEnumerable<User> GetUsers()
        {
            IEnumerable<User> users = _entityFramework.Users.ToList<User>();
            return users;
        }



        public User? GetUserById(Guid userId)
        {
            return _entityFramework.Users.FirstOrDefault(u => u.Id == userId);
        }

        public bool UpdateUserProfile(Guid userId, UserProfileUpdateDto dto)
        {
            var user = _entityFramework.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;

            // Frissítjük a mezőket
            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.PostalCode = dto.PostalCode;
            user.City = dto.City;
            user.Address = dto.Address;

            return _entityFramework.SaveChanges() > 0;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _entityFramework.Users.ToList();
        }

        public bool AdminResetPassword(Guid userId, string newPassword)
        {
            var user = _entityFramework.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;

            // Újra titkosítjuk a jelszót (ugyanaz a logika, mint az AuthRepository-ban)
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            return _entityFramework.SaveChanges() > 0;
        }

        public bool UpdateUserRole(Guid userId, string newRole)
        {
            var user = _entityFramework.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;

            user.Role = newRole;
            return _entityFramework.SaveChanges() > 0;
        }

        // Segédmetódus a jelszó titkosításához
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}