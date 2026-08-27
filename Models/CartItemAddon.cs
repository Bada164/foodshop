namespace foodshop.Models
{

    public class CartItemAddon
    {
        public int Id { get; set; }


        public int CartItemId { get; set; }
        public CartItem? CartItem { get; set; }

        public int AddonId { get; set; }
        public Addon? Addon { get; set; }
    }
}