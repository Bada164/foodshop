using foodshop.DTOs;
using foodshop.Models;
using Microsoft.EntityFrameworkCore;

namespace foodshop.Data
{
    public class CartRepository : ICartRepository
    {
        DataContext _entityFramework;

        private readonly IConfiguration _config;

        public CartRepository(IConfiguration config)
        {
            _entityFramework = new DataContext(config);
            _config = config;
        }

        public Cart GetCartByUserId(Guid userId)
        {
            var cart = _entityFramework.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                // ÚJ: Behúzzuk a feltéteket is!
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Addons)
                        .ThenInclude(cia => cia.Addon)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _entityFramework.Carts.Add(cart);
                _entityFramework.SaveChanges();
            }

            return cart;
        }

        public void AddOrUpdateItemInCart(Guid userId, int productId, int quantity)
        {
            var cart = GetCartByUserId(userId);

            // Megnézzük, van-e már ilyen termék a kosarában
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
            {
                // Ha van, csak növeljük a mennyiséget
                existingItem.Quantity += quantity;
            }
            else if (quantity > 0)
            {
                // Ha nincs, akkor új tételként felvesszük
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                _entityFramework.CartItems.Add(newItem);
            }

            _entityFramework.SaveChanges();
        }

        public void RemoveItemFromCart(Guid userId, int cartItemId)
        {
            var cart = GetCartByUserId(userId);
            // Itt a változás: ci.ProductId helyett ci.Id alapján keresünk!
            var itemToRemove = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);

            if (itemToRemove != null)
            {
                _entityFramework.CartItems.Remove(itemToRemove);
                _entityFramework.SaveChanges();
            }
        }

        public void ClearCart(Guid userId)
        {
            var cart = GetCartByUserId(userId);
            if (cart.CartItems.Any())
            {
                _entityFramework.CartItems.RemoveRange(cart.CartItems);
                _entityFramework.SaveChanges();
            }
        }

        public void AddToCart(Guid userId, CartItemUpdateDto dto)
        {
            var cart = GetCartByUserId(userId);

            // Megkeressük, van-e PONTOSAN ugyanilyen termék a kosárban 
            // (ugyanaz a termék ID, ugyanaz a megjegyzés, ugyanazok a feltétek)
            var existingItem = cart.CartItems.FirstOrDefault(ci =>
                ci.ProductId == dto.ProductId &&
                ci.SpecialInstructions == dto.SpecialInstructions &&
                ci.Addons.Select(a => a.AddonId).OrderBy(id => id).SequenceEqual(dto.AddonIds.OrderBy(id => id))
            );

            if (existingItem != null)
            {
                // Ha pontosan ugyanilyet kért már, csak növeljük a mennyiséget
                existingItem.Quantity += dto.Quantity;
            }
            else if (dto.Quantity > 0)
            {
                // Ha ez egy új/egyedi variáció, létrehozunk egy új sort
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    SpecialInstructions = dto.SpecialInstructions,
                    Addons = new List<CartItemAddon>()
                };

                // Hozzáadjuk az aktív feltéteket
                foreach (var addonId in dto.AddonIds)
                {
                    var addonExists = _entityFramework.Addons.Any(a => a.Id == addonId && a.IsActive);
                    if (addonExists)
                    {
                        newItem.Addons.Add(new CartItemAddon { AddonId = addonId });
                    }
                }

                _entityFramework.CartItems.Add(newItem);
            }

            _entityFramework.SaveChanges();
        }

        public void DecreaseQuantity(Guid userId, int cartItemId)
        {
            var cart = GetCartByUserId(userId);
            // Itt is: ci.ProductId helyett ci.Id alapján keresünk!
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);

            if (existingItem != null)
            {
                if (existingItem.Quantity > 1)
                {
                    existingItem.Quantity -= 1;
                }
                else
                {
                    _entityFramework.CartItems.Remove(existingItem);
                }
                _entityFramework.SaveChanges();
            }
        }
        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }
    }
}