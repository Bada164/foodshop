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
public class CategoriesController : ControllerBase
{
    ICategoryRepository _categoryRepository;
    private readonly IConfiguration _config;

    public CategoriesController(ICategoryRepository categoryRepository, IConfiguration config)
    {
        _categoryRepository = categoryRepository;
        _config = config;
    }


    [HttpGet]
    public IActionResult GetAll()
    {
        var categories = _categoryRepository.GetAllCategories();
        var result = categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        });
        return Ok(result);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Create(CategoryCreateUpdateDto dto)
    {
        var category = _categoryRepository.CreateCategory(dto);
        return Ok(category);
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Update(int id, CategoryCreateUpdateDto dto)
    {
        var success = _categoryRepository.UpdateCategory(id, dto);
        if (!success) return NotFound("Kategória nem található.");
        return Ok(new { message = "Kategória sikeresen frissítve." });
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        try
        {
            var success = _categoryRepository.DeleteCategory(id);
            if (!success) return NotFound("Kategória nem található.");
            return Ok(new { message = "Kategória sikeresen törölve." });
        }
        catch (Exception ex)
        {

            return BadRequest(new { message = ex.Message });
        }
    }
}