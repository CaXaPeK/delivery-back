using System.ComponentModel.DataAnnotations;

namespace Delivery_Service.Schemas
{
    public class DishBasketDto
    {
        public Guid id { get; set; }

        [Required]
        [MinLength(1)]
        public string name { get; set; }

        [Required]
        public double price { get; set; }

        [Required]
        public double totalPrice { get; set; }

        [Required]
        public int amount { get; set; }

        public string? image { get; set; }
    }
}
