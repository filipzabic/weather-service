using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using WeatherService.API.Controllers;
using WeatherService.API.Models;
using WeatherService.API.Services;

namespace WeatherService.API.Tests.Controllers
{
    public class WeatherForecastControllerTests
    {
        [Fact]
        public void Get_ReturnsCorrectNumberOfForecasts()
        {
            // Arrange
            var mockService = new Mock<IWeatherForecastService>();
            mockService.Setup(service => service.GetForecasts())
                .Returns(GetSampleForecasts(5));
            
            var controller = new WeatherForecastController(mockService.Object);

            // Act
            var result = controller.Get();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<WeatherForecast>>>(result);
            var forecasts = Assert.IsAssignableFrom<IEnumerable<WeatherForecast>>(
                ((OkObjectResult)actionResult.Result).Value);
            Assert.Equal(5, forecasts.Count());
        }

        private IEnumerable<WeatherForecast> GetSampleForecasts(int count)
        {
            var rng = new Random();
            return Enumerable.Range(1, count).Select(index => new WeatherForecast
            {
                Id = index,
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = GetRandomSummary(rng)
            })
            .ToArray();
        }
    }
}