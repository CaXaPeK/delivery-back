using Delivery_Service.Context;
using Delivery_Service.Schemas.Classes;
using Delivery_Service.Schemas.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
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

        private double GetBasketPrice()
        {
            var dishInCarts = _context.DishInCarts.Where(x => x.OrderId == null).ToList();

            double sum = 0;
            foreach (var dishInCart in dishInCarts)
            {
                var dish = _context.Dishes.Where(x => x.Id == dishInCart.DishId).First();
                sum += dish.Price * dishInCart.Count;
            }

            return sum;
        }

        private bool IsBasketEmpty()
        {
            if (_context.DishInCarts.Where(x => x.UserId == GetUserIdFromToken() && x.OrderId == null).Count() == 0)
            {
                return true;
            }
            return false;
        }

        private bool OrderAlreadyDelivered(int id)
        {
            if (_context.Orders.Where(x => x.Id == id).First().Status == "Delivered")
            {
                return true;
            }
            return false;
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

        private int NewOrderId()
        {
            if (_context.Orders.Count() > 0)
            {
                return _context.Orders.OrderByDescending(x => x.Id).Select(x => x.Id).First() + 1;
            }

            return 0;
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

        [Authorize]
        [HttpPost]
        public IActionResult create()
        {
            if (IsTokenBad())
            {
                return Forbid();
            }

            if (IsBasketEmpty())
            {
                Response response = new Response
                {
                    status = "Ошибка",
                    message = "Корзина пуста."
                };

                return BadRequest(response);
            }

            int newId = NewOrderId();

            var order = new Order
            {
                Id = newId,
                OrderDate = DateOnly.FromDateTime(DateTime.UtcNow),
                OrderTime = TimeOnly.FromDateTime(DateTime.UtcNow),
                DeliveryDate = null,
                DeliveryTime = null,
                Price = GetBasketPrice(),
                AddressId = _context.Users.Where(x => x.Id == GetUserIdFromToken()).First().Address,
                Status = "InProcess"
            };

            _context.Add(order);
            _context.SaveChanges();

            var dishesInBasket = _context.DishInCarts.Where(x => x.OrderId == null).ToList();

            foreach (var dish in dishesInBasket)
            {
                var dishDb = _context.DishInCarts.Where(x => x.Id == dish.Id).First();
                dishDb.OrderId = newId;
                _context.SaveChanges();
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("{id}/status")]
        public IActionResult updateStatus(int id)
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

            if (OrderAlreadyDelivered(id))
            {
                Response response = new Response
                {
                    status = "Ошибка",
                    message = "Заказ уже доставлен."
                };

                return BadRequest(response);
            }

            var order = _context.Orders.Where(x => x.Id == id).First();

            order.Status = "Delivered";
            order.DeliveryDate = DateOnly.FromDateTime(DateTime.UtcNow);
            order.DeliveryTime = TimeOnly.FromDateTime(DateTime.UtcNow);

            return Ok();
        }
    }
}
