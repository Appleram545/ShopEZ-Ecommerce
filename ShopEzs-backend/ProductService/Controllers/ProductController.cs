using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.DTOs;
using ProductService.Services;

namespace ProductService.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController : ControllerBase
{
    private readonly IProductService _svc;

    public ProductController(IProductService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(await _svc.GetAll(search, page, pageSize));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        try { return Ok(await _svc.GetById(id)); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Add(ProductDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Ok(await _svc.Add(dto));
    }

    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try { return Ok(await _svc.Update(id, dto)); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try { return Ok(await _svc.Delete(id)); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
    }
}
