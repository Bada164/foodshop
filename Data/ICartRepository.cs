using foodshop.DTOs;
using foodshop.Models;

namespace foodshop.Data
{
    public interface ICartRepository
    {
        Cart GetCartByUserId(Guid userId);
        void AddOrUpdateItemInCart(Guid userId, int productId, int quantity);
        void RemoveItemFromCart(Guid userId, int cartItemId);
        void DecreaseQuantity(Guid userId, int cartItemId);
        void ClearCart(Guid userId);

        void AddToCart(Guid userId, CartItemUpdateDto dto);


        public bool SaveChanges();
    }
}