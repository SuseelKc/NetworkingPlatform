using NetworkingPlatform.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetworkingPlatform.Models
{
    public class Posts
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string? Category { get; set; }

        public DateTime? Date { get; set; }

        [ForeignKey("Users")]
        public string users_id { get; set; }

        // Add navigation property for the User
        //public Users User { get; set; }

    }
}
