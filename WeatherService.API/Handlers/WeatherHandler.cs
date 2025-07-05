using Microsoft.EntityFrameworkCore;
using WeatherService.Data;
using WeatherService.Models;

namespace Handlers;

public class WeatherHandler : IWeatherHandler
{
    private readonly HttpClient _httpClient;
    private readonly WeatherContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeatherHandler> _logger;

    public WeatherHandler(
        HttpClient httpClient,
        WeatherContext dbContext,
        IConfiguration configuration,
        ILogger<WeatherHandler> logger)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<OpenMeteoResponse> GetWeatherForecast(DateTime from, DateTime to)
    {
        try
        {
            var latitude = _configuration.GetValue<double>("OpenMeteo:Latitude");
            var longitude = _configuration.GetValue<double>("OpenMeteo:Longitude");
            var baseUrl = _configuration.GetValue<string>("OpenMeteo:BaseUrl");

            string weatherApiUrl = $"{baseUrl}?latitude={latitude}&longitude={longitude}&hourly=temperature_2m&start_date={from:yyyy-MM-dd}&end_date={to:yyyy-MM-dd}";
            
            _logger.LogInformation("Fetching weather data from {Url}", weatherApiUrl);
            
            var weatherData = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(weatherApiUrl) 
                ?? throw new HttpRequestException("Failed to get weather data from API");

            var weatherEntities = weatherData.Hourly.Time
                .Select((time, index) => new WeatherData
                {
                    Date = DateTime.Parse(time),
                    Temperature = weatherData.Hourly.Temperature2M[index],
                })
                .ToList();

            await SaveWeatherData(weatherEntities, from, to);

            return weatherData;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching weather data from API");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while processing weather data");
            throw;
        }
    }

    private async Task SaveWeatherData(List<WeatherData> weatherEntities, DateTime from, DateTime to)
    {
        try
        {
            // Get existing weather data for the same date range to check for duplicates
            var existingDates = await _dbContext.Weather
                .Where(w => w.Date >= from && w.Date <= to)
                .Select(w => w.Date)
                .ToListAsync();

            // Only add weather data that doesn't exist in the database
            var newWeatherEntities = weatherEntities
                .Where(w => !existingDates.Contains(w.Date))
                .ToList();

            if (newWeatherEntities.Count == 0)
            {
                _logger.LogInformation("No new weather data to save");
                return;
            }

            _logger.LogInformation("Saving {Count} new weather records", newWeatherEntities.Count);
            _dbContext.Weather.AddRange(newWeatherEntities);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving weather data to database");
            throw;
        }
    }
}