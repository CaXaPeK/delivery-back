using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Delivery_Service.Schemas
{
    public class DishDto
    {
        public Guid id { get; set; }

        [Required]
        [MinLength(1)]
        public string name { get; set; }

        public string description { get; set; }

        [Required]
        public double price { get; set; }

        public string image { get; set; }

        public bool vegetarian { get; set; }

        [AllowNull]
        public double? rating { get; set; }

        public DishCategory category { get; set; }
    }
}
