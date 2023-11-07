using Delivery_Service.Context;
using Delivery_Service.Schemas.Classes;
using Microsoft.AspNetCore.Mvc;

namespace Delivery_Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly DeliveryDbContext _context;

        public WeatherForecastController(DeliveryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var addresses = _context.AsAddrObjs
                .Where(x => x.Level == "6");

            return Ok(addresses);
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
        public IEnumerable<Response> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new Response
            {
                status = "a",
                message = "b"
            })
            .ToArray();
        }*/
    }
}