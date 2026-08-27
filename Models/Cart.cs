using System.ComponentModel.DataAnnotations.Schema;

namespace foodshop.Models
{
    [Table("carts")]
    public class Cart
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        // Egy kosárhoz több tétel tartozhat
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}