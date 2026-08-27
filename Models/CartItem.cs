using System.ComponentModel.DataAnnotations.Schema;

namespace foodshop.Models
{
    [Table("cart_items")]
    public class CartItem
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("cart_id")]
        public int CartId { get; set; }
        public Cart? Cart { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }


        [Column("special_instructions")]
        public string? SpecialInstructions { get; set; }


        public List<CartItemAddon> Addons { get; set; } = new List<CartItemAddon>();
    }
}