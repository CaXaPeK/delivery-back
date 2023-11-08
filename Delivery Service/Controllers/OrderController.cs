using Delivery_Service.Context;
using Delivery_Service.Schemas.Classes;
using Delivery_Service.Schemas.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Delivery_Service.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly DeliveryDbContext _context;

        public OrderController(DeliveryDbContext context)
        {
            _context = context;
        }

        private bool OrderExists(int id)
        {
            if (_context.Orders.Where(x => x.Id == id).Count() != 0)
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

        private bool IsOrderYours(int id)
        {
            if (_context.Orders.Where(x => x.Id == id).First().UserId == GetUserIdFromToken())
            {
                return true;
            }
            return false;
        }

        private double OrderPrice(int id)
        {
            var dishInCarts = _context.DishInCarts.Where(x => x.OrderId == id).ToList();

            double sum = 0;
            foreach (var dishInCart in dishInCarts)
            {
                var dish = _context.Dishes.Where(x => x.Id == dishInCart.DishId).First();
                sum += dish.Price * dishInCart.Count;
            }

            return sum;
        }

        private List<DishBasketDto> GetOrderDishes(int id)
        {
            var dishInCarts = _context.DishInCarts.Where(x => x.OrderId == id).ToList();
            var dishBasketDtos = new List<DishBasketDto>();

            foreach (var dishInCart in dishInCarts)
            {
                double price = _context.Dishes.Where(x => x.Id == dishInCart.DishId).First().Price;

                var dishDto = new DishBasketDto
                {
                    id = dishInCart.DishId,
                    name = _context.Dishes.Where(x => x.Id == dishInCart.DishId).First().Name,
                    price = price,
                    totalPrice = price * dishInCart.Count,
                    amount = dishInCart.Count,
                    image = _context.Dishes.Where(x => x.Id == dishInCart.DishId).First().Photo
                };

                dishBasketDtos.Add(dishDto);
            }

            return dishBasketDtos;
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult getInfo(int id)
        {
            if (IsTokenBad())
            {
                return Forbid();
            }

            if (!OrderExists(id))
            {
                Response response = new Response
                {
                    status = "Ошибка",
                    message = "Заказ с таким номером не существует."
                };

                return NotFound(response);
            }

            if (!IsOrderYours(id))
            {
                return Forbid();
            }

            var order = _context.Orders.Where(x => x.Id == id).First();
            var orderDto = new OrderDto
            {
                id = order.Id,
                deliveryDate = order.DeliveryDate,
                deliveryTime = order.DeliveryTime,
                orderDate = order.OrderDate,
                orderTime = order.OrderTime,
                status = order.Status == "Delivered" ? OrderStatus.Delivered : OrderStatus.InProcess,
                price = order.Price,
                dishes = GetOrderDishes(id),
                address = order.AddressId
            };

            return Ok(orderDto);
        }

        [Authorize]
        [HttpGet]
        public IActionResult orderList()
        {
            if (IsTokenBad())
            {
                return Forbid();
            }

            var orders = _context.Orders.Where(x => x.UserId == GetUserIdFromToken()).ToList();
            var orderInfoDtos = new List<OrderInfoDto>();

            foreach (var order in orders)
            {
                var orderInfoDto = new OrderInfoDto
                {
                    id = order.Id,
                    deliveryDate = order.DeliveryDate,
                    deliveryTime = order.DeliveryTime,
                    orderDate = order.OrderDate,
                    orderTime = order.OrderTime,
                    status = order.Status == "Delivered" ? OrderStatus.Delivered : OrderStatus.InProcess,
                    price = order.Price
                };

                orderInfoDtos.Add(orderInfoDto);
            }

            return Ok(orderInfoDtos);
        }
    }
}
