using foodshop.Data;
using foodshop.DTOs;
using foodshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace foodshop.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    ICartRepository _cartRepository;
    private readonly IConfiguration _config;

    public CartController(ICartRepository cartRepository, IConfiguration config)
    {
        _cartRepository = cartRepository;
        _config = config;

    }
    private Guid GetUserId()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid.TryParse(userIdString, out Guid userId);
        return userId;
    }

    [HttpGet]
    public IActionResult GetMyCart()
    {
        var cart = _cartRepository.GetCartByUserId(GetUserId());

        var result = new
        {
            cart.Id,
            Items = cart.CartItems.Select(ci => new
            {
                ci.Id,
                ci.ProductId,
                ProductName = ci.Product?.Name,
                ImageUrl = ci.Product?.ImageUrl,
                SpecialInstructions = ci.SpecialInstructions, // Új
                BasePrice = ci.Product?.Price ?? 0,


                Addons = ci.Addons.Select(a => new
                {
                    Id = a.AddonId,
                    a.Addon?.Name,
                    a.Addon?.Price
                }),

                ci.Quantity,


                SubTotal = ((ci.Product?.Price ?? 0) + ci.Addons.Sum(a => a.Addon?.Price ?? 0)) * ci.Quantity
            }),

            TotalAmount = cart.CartItems.Sum(ci =>
                ((ci.Product?.Price ?? 0) + ci.Addons.Sum(a => a.Addon?.Price ?? 0)) * ci.Quantity)
        };

        return Ok(result);
    }
    [HttpPost("add")]
    public IActionResult AddToCart(CartItemUpdateDto dto)
    {
        if (dto.Quantity <= 0) return BadRequest(new { message = "A mennyiségnek nagyobbnak kell lennie 0-nál." });


        _cartRepository.AddToCart(GetUserId(), dto);

        return Ok(new { message = "Termék sikeresen a kosárhoz adva!" });
    }

    [HttpDelete("remove/{cartItemId}")]
    public IActionResult RemoveFromCart(int cartItemId)
    {
        _cartRepository.RemoveItemFromCart(GetUserId(), cartItemId);
        return Ok(new { message = "Termék eltávolítva a kosárból." });
    }

    [HttpDelete("clear")]
    public IActionResult ClearCart()
    {
        _cartRepository.ClearCart(GetUserId());
        return Ok(new { message = "A kosár kiürítve." });
    }


    [HttpPost("decrease/{cartItemId}")]
    public IActionResult DecreaseQuantity(int cartItemId)
    {
        _cartRepository.DecreaseQuantity(GetUserId(), cartItemId);
        return Ok(new { message = "Mennyiség sikeresen csökkentve!" });
    }

}