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
public class OrdersController : ControllerBase
{
    IOrderRepository _orderRepository;
    private readonly IConfiguration _config;

    public OrdersController(IOrderRepository orderRepository, IConfiguration config)
    {
        _orderRepository = orderRepository;
        _config = config;
    }

    [HttpPost]
    public IActionResult PlaceOrder()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
        {
            return Unauthorized(new { message = "Érvénytelen vagy hiányzó felhasználói azonosító." });
        }

        try
        {

            var newOrder = _orderRepository.CreateOrder(userId);

            return Ok(new
            {
                message = "Rendelés sikeresen leadva!",
                orderId = newOrder.Id,
                total = newOrder.TotalAmount
            });
        }
        catch (Exception ex)
        {
            var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return BadRequest(new { message = errorMessage });
        }
    }

    [HttpGet("my-orders")]
    public IActionResult GetMyOrders()
    {

        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
        {
            return Unauthorized(new { message = "Érvénytelen felhasználó." });
        }

        var orders = _orderRepository.GetOrdersByUser(userId);

        var result = orders.Select(o => new
        {
            o.Id,
            o.OrderDate,
            o.TotalAmount,
            o.Status,
            Items = o.OrderItems.Select(oi => new
            {
                ProductName = oi.Product?.Name,
                oi.Quantity,
                oi.UnitPrice,
                SubTotal = oi.Quantity * oi.UnitPrice
            })
        });

        return Ok(result);
    }

    [HttpGet("all-orders")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAllOrders()
    {
        var orders = _orderRepository.GetAllOrders();


        var result = orders.Select(o => new
        {
            Id = o.Id,
            CustomerEmail = o.User?.Email ?? "Ismeretlen",

            // ÚJ: Kiszállítási adatok a Kanban kártyára!
            DeliveryName = o.DeliveryName,
            DeliveryPhone = o.DeliveryPhone,
            DeliveryCity = o.DeliveryCity,
            DeliveryAddress = o.DeliveryAddress,

            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            Status = o.Status,
            Items = o.OrderItems.Select(oi => new
            {
                ProductName = oi.Product?.Name,
                Quantity = oi.Quantity
            })
        });

        return Ok(result);
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateOrderStatus(int id, OrderStatusUpdateDto statusDto)
    {

        var validStatuses = new[] { "Pending", "Preparing", "Delivering", "Completed" };

        if (!validStatuses.Contains(statusDto.Status))
        {
            return BadRequest(new { message = "Érvénytelen státusz. Választható: Pending, Preparing, Delivering, Completed" });
        }

        var success = _orderRepository.UpdateOrderStatus(id, statusDto.Status);

        if (!success)
        {
            return NotFound(new { message = "A rendelés nem található." });
        }

        return Ok(new { message = $"Rendelés státusza sikeresen frissítve erre: {statusDto.Status}" });
    }

}