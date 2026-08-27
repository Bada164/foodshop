using foodshop.DTOs;
using foodshop.Models;

namespace foodshop.Data
{
    public interface IUserRepository
    {

        public bool SaveChanges();
        public IEnumerable<User> GetUsers();

        User? GetUserById(Guid userId);
        bool UpdateUserProfile(Guid userId, UserProfileUpdateDto dto);


        IEnumerable<User> GetAllUsers();
        bool AdminResetPassword(Guid userId, string newPassword);
        bool UpdateUserRole(Guid userId, string newRole);
    }
}