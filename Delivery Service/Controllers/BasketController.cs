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

        private bool DishExists(int id)
        {
            if (_context.Dishes.Where(x => x.Id == id).Count() != 0)
            {
                return true;
            }
            return false;
        }

        private bool DishAlreadyInCart(int id)
        {
            if (_context.DishInCarts.Where(x => x.DishId == id).Count() != 0)
            {
                return true;
            }
            return false;
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

        private int NewDishInCartId()
        {
            if (_context.DishInCarts.Count() > 0)
            {
                return _context.DishInCarts.OrderByDescending(x => x.Id).Select(x => x.Id).First() + 1;
            }

            return 0;
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

        [Authorize]
        [HttpPost("/dish/{dishId}")]
        public IActionResult add(int dishId)
        {
            if (!DishExists(dishId))
            {
                Response response = new Response
                {
                    status = "Ошибка",
                    message = "Блюдо с таким номером не существует."
                };

                return NotFound(response);
            }

            if (DishAlreadyInCart(dishId))
            {
                int dishInCartId = _context.DishInCarts.Where(x => x.DishId == dishId).First().Id;
                DishInCart dishInCart = _context.DishInCarts.Find(dishInCartId);

                dishInCart.Count++;

                _context.SaveChanges();
            }
            else
            {
                var dishInCart = new DishInCart
                {
                    Id = NewDishInCartId(),
                    UserId = GetUserIdFromToken(),
                    DishId = dishId,
                    OrderId = null,
                    Count = 1
                };

                _context.Add(dishInCart);
                _context.SaveChanges();
            }

            return Ok();
        }

        [Authorize]
        [HttpDelete("/dish/{dishId}")]
        public IActionResult delete(int dishId, bool increase)
        {
            if (!DishExists(dishId))
            {
                Response response = new Response
                {
                    status = "Ошибка",
                    message = "Блюдо с таким номером не существует."
                };

                return NotFound(response);
            }

            if (!DishAlreadyInCart(dishId))
            {
                Response response = new Response
                {
                    status = "Ошибка",
                    message = "В корзине нет блюда с таким номером."
                };

                return NotFound(response);
            }

            if (increase)
            {
                var dishCount = _context.DishInCarts.Where(x => x.DishId == dishId).First().Count;

                if (dishCount >= 1)
                {
                    Response response = new Response
                    {
                        status = "Ошибка",
                        message = "Блюдо некуда ещё уменьшать в количестве."
                    };

                    return BadRequest(response);
                }

                int dishInCartId = _context.DishInCarts.Where(x => x.DishId == dishId).First().Id;
                DishInCart dishInCart = _context.DishInCarts.Find(dishInCartId);

                dishInCart.Count--;

                _context.SaveChanges();
            }
            else
            {
                int dishInCartId = _context.DishInCarts.Where(x => x.DishId == dishId).First().Id;
                DishInCart dishInCart = _context.DishInCarts.Find(dishInCartId);

                _context.DishInCarts.Remove(dishInCart);
                _context.SaveChanges();
            }

            return Ok();
        }
    }
}
