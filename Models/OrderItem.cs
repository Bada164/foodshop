using System.ComponentModel.DataAnnotations.Schema;

namespace foodshop.Models
{
    [Table("order_items")] // Itt mondjuk meg neki, hogy aláhúzással keresse!
    public class OrderItem
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit_price")]
        public int UnitPrice { get; set; }

        [Column("special_instructions")]
        public string? SpecialInstructions { get; set; }

        public List<OrderItemAddon> Addons { get; set; } = new List<OrderItemAddon>();
    }
}