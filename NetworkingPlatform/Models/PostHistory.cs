using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NetworkingPlatform.Models
{
    public class PostHistory
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string? Category { get; set; }

        [ForeignKey("Posts")]

        public int post_id { get; set; }

        public DateTime? Date { get; set; }

        [ForeignKey("Users")]
        public string users_id { get; set; }

        public Boolean isDeleted { get; set; } = false;


    }
}
