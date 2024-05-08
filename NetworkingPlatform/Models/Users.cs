using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NetworkingPlatform.Models
{


    public class Users : IdentityUser
    {
        //[Key]
        //public int ID { get; set; }
        //public string Name { get; set; }
        //public string ContactNo { get; set; }
        //public string Email { get; set; }
        //public string Password { get; set; }
        //public UserRole userRole { get; set; }

        public string? Image { get; set; }
        [Required]
        public int UserRole { get; set; }

    }


}
