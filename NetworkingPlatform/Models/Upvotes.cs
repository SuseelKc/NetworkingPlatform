using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NetworkingPlatform.Models
{
    public class Upvotes
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Posts")]
        public int post_id { get; set; }
        [ForeignKey("AspNetUsers")]
        public string users_id { get; set; }
    }
}
