using foodshop.Models;
using foodshop.DTOs;

namespace foodshop.Data
{
    public interface IAuthRepository
    {
        public User Register(UserRegisterDto userRegisterDto);


        public User? Login(UserLoginDto userLoginDto);

        public bool SaveChanges();

        void RevokeRefreshToken(string email);
    }
}