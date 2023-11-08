using Delivery_Service.Schemas.Enums;
using System.ComponentModel.DataAnnotations;

namespace Delivery_Service.Schemas.Classes
{
    public class OrderDto
    {
        public int id { get; set; }

        [Required]
        public DateOnly deliveryDate { get; set; }

        [Required]
        public DateOnly orderDate { get; set; }

        [Required]
        public TimeOnly deliveryTime { get; set; }

        [Required]
        public TimeOnly orderTime { get; set; }

        [Required]
        public OrderStatus status { get; set; }

        [Required]
        public double price { get; set; }

        [Required]
        public List<DishBasketDto> dishes { get; set; }

        [Required]
        [MinLength(1)]
        public string address { get; set; }
    }
}
