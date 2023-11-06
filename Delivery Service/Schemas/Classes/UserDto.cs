using Delivery_Service.Schemas.Enums;
using System.ComponentModel.DataAnnotations;

namespace Delivery_Service.Schemas.Classes
{
    public class UserDto
    {
        public Guid id { get; set; }

        [Required]
        [MinLength(1)]
        public string fullName { get; set; }

        public DateTime birthDate { get; set; }

        [Required]
        public Gender gender { get; set; }

        public Guid address {  get; set; }

        [Required]
        [EmailAddress]
        [MinLength(1)]
        public string email { get; set; }

        [Phone]
        public string phoneNumber { get; set; }
    }
}
