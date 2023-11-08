using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Delivery_Service.Schemas.Enums;

namespace Delivery_Service.Schemas.Classes
{
    public class DishDto
    {
        public int id { get; set; }

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
