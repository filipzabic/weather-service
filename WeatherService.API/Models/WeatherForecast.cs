using System.Text.Json.Serialization;

namespace WeatherService.Models;

public class OpenMeteoResponse
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }
    
    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
    
    [JsonPropertyName("timezone")]
    public string Timezone { get; set; } = string.Empty;
    
    [JsonPropertyName("timezone_abbreviation")]
    public string TimezoneAbbreviation { get; set; } = string.Empty;
    
    [JsonPropertyName("elevation")]
    public double Elevation { get; set; }
    
    [JsonPropertyName("hourly_units")]
    public HourlyUnits HourlyUnits { get; set; } = new();
    
    [JsonPropertyName("hourly")]
    public HourlyData Hourly { get; set; } = new();
}

public class HourlyUnits
{
    [JsonPropertyName("time")]
    public string Time { get; set; } = string.Empty;
    
    [JsonPropertyName("temperature_2m")]
    public string Temperature2m { get; set; } = string.Empty;
}

public class HourlyData
{
    [JsonPropertyName("time")]
    public string[] Time { get; set; } = Array.Empty<string>();
    
    [JsonPropertyName("temperature_2m")]
    public double[] Temperature2M { get; set; } = Array.Empty<double>();
}