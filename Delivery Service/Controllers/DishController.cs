using Delivery_Service.Context;
using Delivery_Service.Schemas.Classes;
using Delivery_Service.Schemas.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Delivery_Service.Controllers
{
    [ApiController]
    [Route("api/dish")]
    public class DishController : ControllerBase
    {
        private readonly DeliveryDbContext _context;

        public DishController(DeliveryDbContext context)
        {
            _context = context;
        }

        private DishCategory GetCategory(string category)
        {
            switch (category)
            {
                case "Wok":
                    return DishCategory.Wok;
                case "Pizza":
                    return DishCategory.Pizza;
                case "Soup":
                    return DishCategory.Soup;
                case "Dessert":
                    return DishCategory.Dessert;
                case "Drink":
                    return DishCategory.Drink;
                default:
                    return DishCategory.Wok;
            }
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

        private double? GetDishRating(int dishId)
        {
            var ratings = _context.Ratings.Where(x => x.DishId == dishId).Select(x => x.Rating1).ToList();
            if (ratings.Count == 0)
            {
                return null;
            }
            else
            {
                double averageRating = 0;
                foreach (var rating in ratings)
                {
                    averageRating += rating;
                }
                return averageRating / (double)ratings.Count;
            }
        }

        private bool DishExists(int id)
        {
            if (_context.Dishes.Where(x => x.Id == id).Count() != 0)
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

        private bool DishEverDelivered(int id)
        {
            var userId = GetUserIdFromToken();

            if (_context.DishInCarts
                .Where(dishInCart => _context.Orders.Any(
                    order => dishInCart.DishId == id &&
                    dishInCart.UserId == userId &&
                    order.Id == dishInCart.OrderId &&
                    order.Status == OrderStatus.Delivered.ToString()
                )).Count() != 0)
            {
                return true;
            }

            return false;
        }

        private bool DishRated(int id)
        {
            if (_context.Ratings.Where(x => x.DishId == id && x.UserId == GetUserIdFromToken()).Count() != 0)
            {
                return true;
            }
            return false;
        }

        [HttpGet]
        public IActionResult list([FromQuery] DishCategory[] categories, bool vegetarian, DishSorting sorting, int page)
        {
            var result = new List<Dish>();

            foreach (DishCategory category in categories)
            {
                var dishes = new List<Dish>();
                var dishesQuery = _context.Dishes.Where(x => x.Category == category.ToString() && x.IsVegetarian == vegetarian);

                switch (sorting)
                {
                    case DishSorting.NameAsc:
                        dishes = dishesQuery.OrderBy(x => x.Name).ToList();
                        break;
                    case DishSorting.NameDesc:
                        dishes = dishesQuery.OrderByDescending(x => x.Name).ToList();
                        break;
                    case DishSorting.PriceAsc:
                        dishes = dishesQuery.OrderBy(x => x.Price).ToList();
                        break;
                    case DishSorting.PriceDesc:
                        dishes = dishesQuery.OrderByDescending(x => x.Price).ToList();
                        break;
                    case DishSorting.RatingAsc:
                        dishes = dishesQuery.OrderBy(x => GetDishRating(x.Id)).Where(x => GetDishRating(x.Id) != null).ToList();
                        break;
                    case DishSorting.RatingDesc:
                        dishes = dishesQuery.OrderByDescending(x => GetDishRating(x.Id)).Where(x => GetDishRating(x.Id) != null).ToList();
                        break;
                    default:
                        dishes = dishesQuery.ToList();
                        break;
                }

                result.AddRange(dishes);
            }

            int pageSize = 5;
            PageInfoModel pageInfo = new PageInfoModel
            {
                size = pageSize,
                count = (int)Math.Ceiling((double)(result.Count / pageSize)) + 1,
                current = page
            };

            if (page < 1 || page > pageInfo.count)
            {
                Response response = new Response
                {
                    status = "Ошибка",
                    message = "Недостижимая страница."
                };

                return BadRequest(response);
            }

            var resultDtos = new List<DishDto>();
            for (int i = 0; i < result.Count; i++)
            {
                if (i / pageSize + 1 != page)
                {
                    continue;
                }

                Dish dish = result[i];
                DishDto dishDto = new DishDto
                {
                    id = dish.Id,
                    name = dish.Name,
                    description = dish.Description,
                    price = dish.Price,
                    image = dish.Photo,
                    vegetarian = (bool)dish.IsVegetarian,
                    rating = GetDishRating(dish.Id),
                    category = GetCategory(dish.Category)
                };

                resultDtos.Add(dishDto);
            }

            var pagedList = new DishPagedListDto
            {
                dishes = resultDtos,
                pagination = pageInfo
            };

            return Ok(pagedList);
        }
        private int NewRatingId()
        {
            if (_context.Ratings.Count() > 0)
            {
                return _context.Ratings.OrderByDescending(x => x.Id).Select(x => x.Id).First() + 1;
            }

            return 0;
        }

        [HttpGet("{id}")]
        public IActionResult dish(int id)
        {
            if (!DishExists(id))
            {
                Response response = new Response
                {
                    status = "Ошибка",
                    message = "Блюдо с таким номером не существует."
                };

                return NotFound(response);
            }

            var dish = _context.Dishes.Where(x => x.Id == id).First();
            var dishDto = new DishDto
            {
                id = dish.Id,
                name = dish.Name,
                description = dish.Description,
                price = dish.Price,
                image = dish.Photo,
                vegetarian = (bool)dish.IsVegetarian,
                rating = GetDishRating(dish.Id),
                category = GetCategory(dish.Category)
            };
            
            return Ok(dishDto);
        }

        [Authorize]
        [HttpGet("{id}/rating/check")]
        public IActionResult ratingCheck(int id)
        {
            if (IsTokenBad())
            {
                return Forbid();
            }

            if (!DishExists(id))
            {
                Response response = new Response
                {
                    status = "Ошибка",
                    message = "Блюдо с таким номером не существует."
                };

                return NotFound(response);
            }
            
            return Ok(DishEverDelivered(id));
        }

        [Authorize]
        [HttpPost("{id}/rating")]
        public IActionResult ratingSet(int id, int ratingScore)
        {
            if (!DishExists(id))
            {
                Response response = new Response
                {
                    status = "Ошибка",
                    message = "Блюдо с таким номером не существует."
                };

                return NotFound(response);
            }

            if (!DishEverDelivered(id))
            {
                Forbid();
            }

            if (DishRated(id))
            {
                var rating = _context.Ratings.Where(x => x.DishId == id).First();
                rating.Rating1 = ratingScore;

                _context.SaveChanges();
            }
            else
            {
                var rating = new Rating
                {
                    Id = NewRatingId(),
                    Rating1 = ratingScore, //я случайно назвал значение оценки не Value, а Rating1
                    UserId = GetUserIdFromToken(),
                    DishId = id
                };

                _context.Add(rating);
                _context.SaveChanges();
            }

            

            return Ok();
        }
    }
}
