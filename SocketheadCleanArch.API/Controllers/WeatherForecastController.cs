using Microsoft.AspNetCore.Mvc;
using SocketheadCleanArch.Domain.Dtos;

namespace SocketheadCleanArch.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(
    //ILogger<WeatherForecastController> logger
    ) : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecastDto> Get()
    {
        return Enumerable
            .Range(1, 5)
            .Select(index => new WeatherForecastDto
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}