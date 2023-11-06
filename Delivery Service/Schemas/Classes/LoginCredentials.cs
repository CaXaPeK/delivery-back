using System.ComponentModel.DataAnnotations;

namespace Delivery_Service.Schemas.Classes
{
    public class LoginCredentials
    {
        [Required]
        [EmailAddress]
        [MinLength(1)]
        public string email { get; set; }

        [Required]
        [MinLength(1)]
        public string password { get; set; }
    }
}
