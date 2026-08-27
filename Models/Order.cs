using System.ComponentModel.DataAnnotations.Schema; // Ez kell a [Table] és [Column] miatt!

namespace foodshop.Models
{
    [Table("orders")] // Pontosan így hívják az adatbázisban
    public class Order
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column("total_amount")]
        public int TotalAmount { get; set; }

        [Column("status")]
        public string Status { get; set; } = "Pending";

        [Column("delivery_name")]
        public string? DeliveryName { get; set; }

        [Column("delivery_phone")]
        public string? DeliveryPhone { get; set; }

        [Column("delivery_city")]
        public string? DeliveryCity { get; set; }

        [Column("delivery_address")]
        public string? DeliveryAddress { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}