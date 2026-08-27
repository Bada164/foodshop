using foodshop.DTOs;
using foodshop.Models;
using Microsoft.EntityFrameworkCore;

namespace foodshop.Data
{
    public class OrderRepository : IOrderRepository
    {
        DataContext _entityFramework;

        private readonly IConfiguration _config;

        public OrderRepository(IConfiguration config)
        {
            _entityFramework = new DataContext(config);
            _config = config;
        }


        public Order CreateOrder(Guid userId)
        {

            var user = _entityFramework.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null || string.IsNullOrEmpty(user.Address) || string.IsNullOrEmpty(user.City) || string.IsNullOrEmpty(user.PhoneNumber))
            {
                throw new Exception("Kérjük, a rendelés leadása előtt töltsd ki a szállítási adataidat (Cím, Város, Telefonszám) a Profilodban!");
            }

            var cart = _entityFramework.Carts
                     .Include(c => c.CartItems)
                         .ThenInclude(ci => ci.Product)
                     .Include(c => c.CartItems)
                         .ThenInclude(ci => ci.Addons) // Behúzzuk a feltéteket!
                             .ThenInclude(cia => cia.Addon)
                     .FirstOrDefault(c => c.UserId == userId);


            if (cart == null || !cart.CartItems.Any())
            {
                throw new Exception("A kosár üres, nem lehet rendelést leadni!");
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = 0,
                DeliveryName = user.FullName ?? user.Email, // Ha nincs név, az e-mailt használjuk
                DeliveryPhone = user.PhoneNumber,
                DeliveryCity = user.City,
                DeliveryAddress = user.Address,
                OrderItems = new List<OrderItem>()
            };


            foreach (var cartItem in cart.CartItems)
            {

                if (cartItem.Product == null || !cartItem.Product.IsActive)
                {
                    throw new Exception($"A termék (ID: {cartItem.ProductId}) már nem rendelhető.");
                }

                int itemTotalUnitPrice = cartItem.Product.Price;

                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price,
                    SpecialInstructions = cartItem.SpecialInstructions, // Másoljuk a megjegyzést!
                    Addons = new List<OrderItemAddon>()
                };


                foreach (var cartAddon in cartItem.Addons)
                {
                    if (cartAddon.Addon != null)
                    {
                        orderItem.Addons.Add(new OrderItemAddon
                        {
                            AddonName = cartAddon.Addon.Name,
                            Price = cartAddon.Addon.Price
                        });

                        // Hozzáadjuk a feltét árát is a tétel egységárához!
                        itemTotalUnitPrice += cartAddon.Addon.Price;
                    }
                }
                order.TotalAmount += (itemTotalUnitPrice * cartItem.Quantity);
                order.OrderItems.Add(orderItem);
            }


            _entityFramework.Orders.Add(order);


            _entityFramework.CartItems.RemoveRange(cart.CartItems);


            _entityFramework.SaveChanges();

            return order;
        }


        public IEnumerable<Order> GetOrdersByUser(Guid userId)
        {
            return _entityFramework.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }


        public IEnumerable<Order> GetAllOrders()
        {
            return _entityFramework.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderBy(o => o.OrderDate)
                .ToList();
        }

        public bool UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = _entityFramework.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return false;

            order.Status = newStatus;
            return _entityFramework.SaveChanges() > 0;
        }
        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }
    }
}