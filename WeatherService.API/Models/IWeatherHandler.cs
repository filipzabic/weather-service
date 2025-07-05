using WeatherService.Models;

namespace Handlers;

public interface IWeatherHandler
{
    Task<OpenMeteoResponse> GetWeatherForecast(DateTime from, DateTime to);
}