using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NetworkingPlatform.Models
{
    public class ReplyVotes
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Reply")]
        public int reply_id { get; set; }
        [ForeignKey("AspNetUsers")]
        public string users_id { get; set; }

        //create votetype enums
        public int voteType { get; set; }
    }
}
