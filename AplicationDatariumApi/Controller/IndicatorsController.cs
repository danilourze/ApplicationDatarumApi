using AplicationDatariumApi.Domain.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AplicationDatariumApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class IndicatorsController : ControllerBase
{
    private readonly IEconomicIndicatorService _econ;
    public IndicatorsController(IEconomicIndicatorService econ) => _econ = econ;

    [HttpGet("selic")]
    public async Task<IActionResult> GetSelic([FromQuery] DateTime? date, CancellationToken ct)
    {
        var v = await _econ.GetSelicDailyAsync(date, ct);
        return v is null ? NotFound() : Ok(new { date = date?.Date, selic = v });
    }

    [HttpGet("selic/history")]
    public async Task<IActionResult> GetSelicHistory([FromQuery] DateTime start, [FromQuery] DateTime end, CancellationToken ct)
    {
        if (end < start) return BadRequest("end < start");
        var data = await _econ.GetSelicSeriesAsync(start, end, ct);
        return Ok(data.Select(x => new { date = x.date, value = x.value }));
    }
}
