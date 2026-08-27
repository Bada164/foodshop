using foodshop.DTOs;
using foodshop.Models;
using Microsoft.EntityFrameworkCore;

namespace foodshop.Data
{
    public class FavoriteRepository : IFavoriteRepository
    {
        DataContext _entityFramework;

        private readonly IConfiguration _config;

        public FavoriteRepository(IConfiguration config)
        {
            _entityFramework = new DataContext(config);
            _config = config;
        }


        public IEnumerable<Favorite> GetFavoritesByUserId(Guid userId)
        {
            // Lekérjük a kedvenceket, és egyből hozzácsapjuk a Termék adatait (Név, Ár, Kép) is!
            return _entityFramework.Favorites
                .Include(f => f.Product)
                .Where(f => f.UserId == userId)
                .ToList();
        }

        public bool AddToFavorites(Guid userId, int productId)
        {
            // Ellenőrizzük, létezik-e egyáltalán a termék
            var productExists = _entityFramework.Products.Any(p => p.Id == productId);
            if (!productExists) return false;

            // Ellenőrizzük, benne van-e már a kedvencekben
            var alreadyFavorite = _entityFramework.Favorites.Any(f => f.UserId == userId && f.ProductId == productId);
            if (alreadyFavorite) return true; // Már benne van, nincs dolgunk

            var favorite = new Favorite
            {
                UserId = userId,
                ProductId = productId
            };

            _entityFramework.Favorites.Add(favorite);
            _entityFramework.SaveChanges();
            return true;
        }

        public bool RemoveFromFavorites(Guid userId, int productId)
        {
            var favorite = _entityFramework.Favorites.FirstOrDefault(f => f.UserId == userId && f.ProductId == productId);
            if (favorite == null) return false;

            _entityFramework.Favorites.Remove(favorite);
            _entityFramework.SaveChanges();
            return true;
        }
    }
}
