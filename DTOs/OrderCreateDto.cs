namespace foodshop.DTOs
{
    public class OrderCreateDto
    {

        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}