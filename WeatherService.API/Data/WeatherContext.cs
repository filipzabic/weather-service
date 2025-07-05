using Microsoft.EntityFrameworkCore;
using WeatherService.Models;

namespace WeatherService.Data;

public class WeatherContext(DbContextOptions options, IConfiguration configuration) : DbContext(options)
{
    public DbSet<WeatherData> Weather { get; set; } = null!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(configuration.GetConnectionString("WebApiDatabase"));
    }
}