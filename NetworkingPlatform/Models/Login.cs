using System.ComponentModel.DataAnnotations;

namespace NetworkingPlatform.Models
{
    public class Login
    {
        public class LoginModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}
