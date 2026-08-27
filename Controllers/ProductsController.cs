using foodshop.Data;
using foodshop.DTOs;
using foodshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodshop.Controllers;


[ApiController]
[Route("[controller]")]

public class ProductsController : ControllerBase
{
    IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [HttpGet("GetAllActiveProducts")]
    public IActionResult GetAllActiveProducts()
    {
        IEnumerable<Product> products = _productRepository.GetAllActiveProducts();
        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            CategoryName = p.Category?.Name ?? "",
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            Allergens = p.Allergens
        });

        return Ok(productDtos);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateProduct(ProductCreateUpdateDto productDto)
    {
        var product = new Product
        {
            CategoryId = productDto.CategoryId,
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            ImageUrl = productDto.ImageUrl,
            Allergens = productDto.Allergens,
            IsActive = productDto.IsActive
        };

        _productRepository.AddProduct(product);

        if (_productRepository.SaveChanges())
        {
            return Ok(new { message = "Termék sikeresen létrehozva!" });
        }

        return BadRequest("Hiba történt a termék mentésekor.");
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateProduct(int id, ProductCreateUpdateDto productDto)
    {

        var existingProduct = _productRepository.GetProductById(id);
        if (existingProduct == null)
        {
            return NotFound(new { message = "A termék nem található." });
        }


        existingProduct.CategoryId = productDto.CategoryId;
        existingProduct.Name = productDto.Name;
        existingProduct.Description = productDto.Description;
        existingProduct.Price = productDto.Price;
        existingProduct.ImageUrl = productDto.ImageUrl;
        existingProduct.Allergens = productDto.Allergens;
        existingProduct.IsActive = productDto.IsActive;

        _productRepository.UpdateProduct(existingProduct);

        if (_productRepository.SaveChanges())
        {
            return Ok(new { message = "Termék sikeresen frissítve!" });
        }

        return BadRequest("Hiba történt a termék frissítésekor.");
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteProduct(int id)
    {
        var product = _productRepository.GetProductById(id);
        if (product == null)
        {
            return NotFound(new { message = "A termék nem található." });
        }

        _productRepository.DeleteProduct(product);

        if (_productRepository.SaveChanges())
        {
            return Ok(new { message = "Termék sikeresen törölve!" });
        }

        return BadRequest("Hiba történt a termék törlésekor.");
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAllProductsForAdmin()
    {
        IEnumerable<Product> products = _productRepository.GetAllProductsAdmin();

        var productDtos = products.Select(p => new
        {
            Id = p.Id,
            Name = p.Name,
            CategoryName = p.Category?.Name ?? "",
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            Allergens = p.Allergens,
            IsActive = p.IsActive
        });

        return Ok(productDtos);
    }


    [HttpPatch("{id}/toggle-status")]
    [Authorize(Roles = "Admin")]
    public IActionResult ToggleProductStatus(int id)
    {

        var product = _productRepository.GetProductById(id);
        if (product == null)
        {
            return NotFound(new { message = "A termék nem található." });
        }


        product.IsActive = !product.IsActive;


        _productRepository.UpdateProduct(product);

        if (_productRepository.SaveChanges())
        {
            var currentStatus = product.IsActive ? "Aktív" : "Inaktív";
            return Ok(new { message = $"Termék állapota sikeresen {currentStatus} lett!" });
        }

        return BadRequest("Hiba történt a státusz módosításakor.");
    }

}


