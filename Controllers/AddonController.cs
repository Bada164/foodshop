using foodshop.Data;
using Microsoft.AspNetCore.Mvc;

namespace foodshop.Controllers;

[ApiController]
[Route("[controller]")]
public class AddonsController : ControllerBase
{
    private readonly IAddonRepository _addonRepository;

    public AddonsController(IAddonRepository addonRepository)
    {
        _addonRepository = addonRepository;
    }

    [HttpGet]
    public IActionResult GetActiveAddons()
    {
        var addons = _addonRepository.GetActiveAddons();

        // DTO-szerűen csak a lényeget küldjük ki a Reactnak
        var result = addons.Select(a => new
        {
            Id = a.Id,
            Name = a.Name,
            Price = a.Price
        });

        return Ok(result);
    }
}