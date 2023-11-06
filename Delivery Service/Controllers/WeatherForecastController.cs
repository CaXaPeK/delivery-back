using Delivery_Service.Schemas;
using Microsoft.AspNetCore.Mvc;

namespace Delivery_Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /*[HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }*/

        /*[HttpGet]
        public IEnumerable<DishBasketDto> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new DishBasketDto
            {
                id = Guid.NewGuid(),
                name = "ads",
                price = 0,
                totalPrice = 0,
                amount = 1,
                image = "poi"
            })
            .ToArray();
        }*/

        [HttpGet]
        public IEnumerable<DishDto> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new DishDto
            {
                id = Guid.NewGuid(),
                name = "ads",
                description = "",
                price = 0,
                image = "poi",
                vegetarian = false,
                rating = 0,
                category = DishCategory.Wok
            })
            .ToArray();
        }
    }
}