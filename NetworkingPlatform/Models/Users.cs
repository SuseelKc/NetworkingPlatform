using System.ComponentModel.DataAnnotations;

namespace NetworkingPlatform.Models
{
    public class Users
    {

        public enum UserRole
        {
            Bloggers = 0,
            Admin = 1
            // Add more roles if needed
        }

        public class Users
        {
            [Key]
            public int ID { get; set; }
            public string Name { get; set; }
            public string ContactNo { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public UserRole userRole { get; set; }

        }

    }
}
