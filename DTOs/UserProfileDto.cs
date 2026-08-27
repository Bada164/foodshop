namespace foodshop.DTOs
{
    public class UserProfileDto
    {
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
    }
}