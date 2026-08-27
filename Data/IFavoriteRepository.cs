using foodshop.Models;

namespace foodshop.Data
{
    public interface IFavoriteRepository
    {
        IEnumerable<Favorite> GetFavoritesByUserId(Guid userId);
        bool AddToFavorites(Guid userId, int productId);
        bool RemoveFromFavorites(Guid userId, int productId);
    }
}