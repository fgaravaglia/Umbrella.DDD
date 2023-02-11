using System;
using Microsoft.AspNetCore.Mvc;
using Umbrella.DDD.Abstractions;
using Umbrella.DDD.WebApi.TestScenarios;

namespace UMbrella.DDD.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    IMessageBus _bus;

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IMessageBus bus)
    {
        _logger = logger;
        _bus = bus ?? throw new System.ArgumentNullException(nameof(bus));
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("publish", Name = "Publish")]
    public string Publish()
    {
        var message = TestEventOccurred.NewEventFrom("id", "prova");
        return this._bus.PublishMessage(message);
    }
}
