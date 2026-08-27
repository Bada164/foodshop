using foodshop.DTOs;
using foodshop.Models;

namespace foodshop.Data
{
    public interface IOrderRepository
    {
        Order CreateOrder(Guid userId);

        IEnumerable<Order> GetOrdersByUser(Guid userId);
        IEnumerable<Order> GetAllOrders();
        bool UpdateOrderStatus(int orderId, string newStatus);
        public bool SaveChanges();
    }
}