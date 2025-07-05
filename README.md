# Weather Service API

A simple .NET 9 Web API for weather data, using Entity Framework Core and SQLite.

## Project Structure

- `WeatherService.API/` - Main ASP.NET Core Web API project
- `WeatherService.API.Tests/` - Unit tests for the API

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQLite

### Running the API

```sh
dotnet run --project WeatherService.API/WeatherService.API.csproj
```

The API will start on the configured port (see `appsettings.json`).

### Running Tests

```sh
dotnet test WeatherService.API.Tests/WeatherService.API.Tests.csproj
```

## API Documentation

Swagger UI is available when running the API at `/swagger`.

## Database

- Uses SQLite (`weather.db` in the API project directory)
- Entity Framework Core handles migrations and schema

## Development

- Update database schema with EF Core migrations:

  ```sh
  dotnet ef migrations add <MigrationName> --project WeatherService.API
  dotnet ef database update --project WeatherService.API
  ```

## License

MIT License
