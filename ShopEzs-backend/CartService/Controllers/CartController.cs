using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CartService.DTOs;
using CartService.Services;
using System.Security.Claims;

namespace CartService.Controllers;

[Authorize]
[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _svc;

    public CartController(ICartService svc) => _svc = svc;

    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (claim == null || !int.TryParse(claim, out int id))
            throw new UnauthorizedAccessException("Invalid token.");
        return id;
    }

    [HttpGet]
    public IActionResult Get()
    {
        try { return Ok(_svc.GetCart(GetUserId())); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
    }

    [HttpPost]
    public async Task<IActionResult> Add(CartItemDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try { return Ok(await _svc.AddItem(GetUserId(), dto)); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        catch (Exception ex) { return StatusCode(500, ex.Message); }
    }

    [HttpDelete("{productId}")]
    public IActionResult Remove(int productId)
    {
        try { return Ok(_svc.RemoveItem(GetUserId(), productId)); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
    }

    [HttpDelete]
    public IActionResult Clear()
    {
        try { return Ok(_svc.Clear(GetUserId())); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
    }
}
