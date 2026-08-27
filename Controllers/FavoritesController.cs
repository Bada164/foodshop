using foodshop.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace foodshop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoritesController(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        private Guid GetUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdString, out Guid userId);
            return userId;
        }

        [HttpGet]
        public IActionResult GetMyFavorites()
        {
            var favorites = _favoriteRepository.GetFavoritesByUserId(GetUserId());


            var result = favorites.Select(f => new
            {
                f.ProductId,
                ProductName = f.Product?.Name,
                Description = f.Product?.Description,
                Price = f.Product?.Price,
                ImageUrl = f.Product?.ImageUrl
            });

            return Ok(result);
        }

        [HttpPost("{productId}")]
        public IActionResult AddFavorite(int productId)
        {
            var success = _favoriteRepository.AddToFavorites(GetUserId(), productId);
            if (!success) return BadRequest(new { message = "A termék nem található." });

            return Ok(new { message = "Hozzáadva a kedvencekhez!" });
        }

        [HttpDelete("{productId}")]
        public IActionResult RemoveFavorite(int productId)
        {
            var success = _favoriteRepository.RemoveFromFavorites(GetUserId(), productId);
            if (!success) return NotFound(new { message = "Nincs a kedvencek között." });

            return Ok(new { message = "Eltávolítva a kedvencekből." });
        }
    }
}