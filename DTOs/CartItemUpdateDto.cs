namespace foodshop.DTOs
{
    public class CartItemUpdateDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public string? SpecialInstructions { get; set; }


        public List<int> AddonIds { get; set; } = new List<int>();
    }
}