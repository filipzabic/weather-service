using System.ComponentModel.DataAnnotations;
using Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherService.Data;
using WeatherService.Models;

namespace WeatherService.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWeatherHandler _weatherHandler;
    private readonly WeatherContext _dbContext;

    public WeatherForecastController(IWeatherHandler weatherHandler, WeatherContext dbContext)
    {
        _weatherHandler = weatherHandler;
        _dbContext = dbContext;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OpenMeteoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OpenMeteoResponse>> AddForecast([FromBody] DateRangeModel dateRange)
    {
        try
        {
            var forecast = await _weatherHandler.GetWeatherForecast(dateRange.From, dateRange.To);
            return Ok(forecast);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Weather service is currently unavailable");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    [HttpGet("historical")]
    [ProducesResponseType(typeof(IEnumerable<WeatherData>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<WeatherData>>> GetHistoricalData([FromQuery] DateRangeModel dateRange)
    {
        var historicalData = await _dbContext.Weather
            .Where(w => w.Date >= dateRange.From && w.Date <= dateRange.To)
            .OrderBy(w => w.Date)
            .ToListAsync();

        return Ok(historicalData);
    }

    [HttpGet("statistics")]
    [ProducesResponseType(typeof(WeatherStatistics), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WeatherStatistics>> GetStatistics([FromQuery] DateRangeModel dateRange)
    {
        var data = await _dbContext.Weather
            .Where(w => w.Date >= dateRange.From && w.Date <= dateRange.To)
            .Select(w => w.Temperature!.Value)
            .ToListAsync();

        if (!data.Any())
        {
            return Ok(new WeatherStatistics());
        }

        var statistics = new WeatherStatistics
        {
            AverageTemperature = data.Average(),
            MinTemperature = data.Min(),
            MaxTemperature = data.Max()
        };

        return Ok(statistics);
    }
}

public class DateRangeModel : IValidatableObject
{
    [Required]
    public DateTime From { get; set; }

    [Required]
    public DateTime To { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (From > To)
        {
            yield return new ValidationResult(
                "From date must be before or equal to To date",
                new[] { nameof(From), nameof(To) });
        }

        if (To.Subtract(From).TotalDays > 7)
        {
            yield return new ValidationResult(
                "Date range cannot exceed 7 days",
                new[] { nameof(From), nameof(To) });
        }

        if (From < DateTime.UtcNow.Date)
        {
            yield return new ValidationResult(
                "From date cannot be in the past",
                new[] { nameof(From) });
        }
    }
}

public class WeatherStatistics
{
    public double AverageTemperature { get; set; }
    public double MinTemperature { get; set; }
    public double MaxTemperature { get; set; }
}