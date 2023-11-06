using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Delivery_Service.Schemas.Classes
{
    public class OrderCreateDto
    {
        [Required]
        public DateTime deliveryTime { get; set; }

        public Guid addressId { get; set; }
    }
}
