using Delivery_Service.Schemas.Enums;
using System.ComponentModel.DataAnnotations;

namespace Delivery_Service.Schemas.Classes
{
    public class UserRegisterModel
    {
        [Required]
        [MinLength(1)]
        public string fullName { get; set; }

        [Required]
        [MinLength(6)]
        public string password { get; set; }

        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string email { get; set; }

        public Guid addressId { get; set; }

        public DateTime birthDate { get; set; }

        [Required]
        public Gender gender { get; set; }

        [Phone]
        public string phoneNumber { get; set; }
    }
}
