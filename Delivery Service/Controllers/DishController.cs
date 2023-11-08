using Delivery_Service.Context;
using Delivery_Service.Schemas.Classes;
using Delivery_Service.Schemas.Enums;
using Microsoft.AspNetCore.Mvc;

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
                return averageRating/(double)ratings.Count;
            }
        }



        [HttpGet]
        public IActionResult list([FromQuery]DishCategory[] categories, bool vegetarian, DishSorting sorting, int page)
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
                count = (int)Math.Ceiling((double)(result.Count / pageSize)),
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
    }
}
