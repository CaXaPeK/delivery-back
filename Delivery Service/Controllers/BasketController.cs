using Delivery_Service.Context;
using Delivery_Service.Schemas.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Delivery_Service.Controllers
{
    [ApiController]
    [Route("api/basket")]
    public class BasketController : ControllerBase
    {
        private readonly DeliveryDbContext _context;

        public BasketController(DeliveryDbContext context)
        {
            _context = context;
        }

        private string GetToken()
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"].ToString();
            string token = authorizationHeader.Substring("Bearer ".Length);

            return token;
        }

        private bool IsTokenBad()
        {
            if (_context.BadTokens.Where(x => x.Value == GetToken()).Count() != 0)
            {
                return true;
            }
            return false;
        }

        private string GetEmailFromToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(GetToken());

            return token.Claims.First(claim => claim.Type == "email").Value;
        }

        private int GetUserIdFromToken()
        {
            var email = GetEmailFromToken();
            return _context.Users.Where(x => x.Email == email).First().Id;
        }

        [Authorize]
        [HttpGet]
        public IActionResult list()
        {
            if (IsTokenBad())
            {
                return Forbid();
            }

            var dishes = _context.DishInCarts.Where(x => x.UserId == GetUserIdFromToken() && x.OrderId == null).ToList();
            var dishDtos = new List<DishBasketDto>();

            foreach (var dish in dishes)
            {
                double price = _context.Dishes.Where(x => x.Id == dish.DishId).First().Price;

                var dishDto = new DishBasketDto
                {
                    id = dish.DishId,
                    name = _context.Dishes.Where(x => x.Id == dish.DishId).First().Name,
                    price = price,
                    totalPrice = price * dish.Count,
                    amount = dish.Count,
                    image = _context.Dishes.Where(x => x.Id == dish.DishId).First().Photo
                };

                dishDtos.Add(dishDto);
            }

            return Ok(dishDtos);
        }
    }
}
