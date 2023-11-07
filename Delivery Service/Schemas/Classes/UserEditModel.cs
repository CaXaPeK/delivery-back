using Delivery_Service.Schemas.Enums;
using System.ComponentModel.DataAnnotations;

namespace Delivery_Service.Schemas.Classes
{
    public class UserEditModel
    {
        [Required]
        [MinLength(1)]
        public string fullName { get; set; }

        public DateOnly birthDate { get; set; }

        [Required]
        public Gender gender { get; set; }

        public Guid addressId { get; set; }

        [Phone]
        public string phoneNumber { get; set; }
    }
}
