using System.ComponentModel.DataAnnotations;

namespace Delivery_Service.Schemas.Classes
{
    public class TokenResponse
    {
        [Required]
        [MinLength(1)]
        public string token { get; set; }
    }
}
