using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NetworkingPlatform.Models
{
    public class PostHistory
    {
        [Key]
        public int ID { get; set; }
        public string Old { get; set; }
        public string New { get; set; }
        public string Description { get; set; }
        public string? Category { get; set; }

        public DateTime? Date { get; set; }

        [ForeignKey("Users")]
        public string users_id { get; set; }


    }
}
