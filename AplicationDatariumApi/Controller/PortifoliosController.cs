using AplicationDatariumApi.Domain;
using AplicationDatariumApi.Domain.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AplicationDatariumApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class PortfoliosController : ControllerBase
{
    private readonly IPortfolioService _service;

    public PortfoliosController(IPortfolioService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Portifolio>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Portifolio>> GetById(long id)
    {
        var entity = await _service.GetByIdAsync(id);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<Portifolio>> Create([FromBody] Portifolio input)
    {
        var created = await _service.CreateAsync(input);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] Portifolio input)
    {
        if (id != input.Id) return BadRequest("Id do corpo difere do parâmetro.");
        await _service.UpdateAsync(input);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    // EXPORT
    [HttpGet("{id:long}/export")]
    public async Task<IActionResult> Export(long id)
    {
        var (content, contentType, fileName) = await _service.ExportAsync(id);
        return File(content, contentType, fileName);
    }


}