using Delivery_Service.Schemas.Enums;
using System.ComponentModel.DataAnnotations;

namespace Delivery_Service.Schemas.Classes
{
    public class OrderInfoDto
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
    }
}
