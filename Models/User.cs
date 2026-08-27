namespace foodshop.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public string Role { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public User()
        {
            if (Email == null)
            {
                Email = "";
            }

            if (Role == null)
            {
                Role = "";
            }
            if (RefreshToken == null)
            {
                RefreshToken = "";
            }
            if (FullName == null)
            {
                FullName = "";
            }
            if (PhoneNumber == null)
            {
                PhoneNumber = "";
            }
            if (PostalCode == null)
            {
                PostalCode = "";
            }
            if (City == null)
            {
                City = "";
            }
            if (Address == null)
            {
                Address = "";
            }
        }
    }
}