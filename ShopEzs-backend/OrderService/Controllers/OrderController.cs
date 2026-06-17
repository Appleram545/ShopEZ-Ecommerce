using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.DTOs;
using OrderService.Services;
using System.Security.Claims;

namespace OrderService.Controllers;

[Authorize]
[ApiController]
[Route("api/order")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _svc;

    public OrderController(IOrderService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create(OrderDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            return Unauthorized("Invalid token.");

        try { return Ok(await _svc.Create(dto, userId)); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        try
        {
            var orders = role?.ToLower() == "admin"
                ? await _svc.GetAll()
                : await _svc.GetByUserId(userId);
            return Ok(orders);
        }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        if (id <= 0) return BadRequest("ID must be > 0.");
        try { return Ok(await _svc.GetById(id)); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }
}
