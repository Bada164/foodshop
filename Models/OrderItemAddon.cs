namespace foodshop.Models
{

    public class OrderItemAddon
    {
        public int Id { get; set; }


        public int OrderItemId { get; set; }
        public OrderItem? OrderItem { get; set; }


        public string AddonName { get; set; } = string.Empty;


        public int Price { get; set; }
    }
}