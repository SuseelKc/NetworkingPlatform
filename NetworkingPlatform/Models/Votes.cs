using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NetworkingPlatform.Enums;

namespace NetworkingPlatform.Models
{
    public class Votes
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Posts")]
        public int post_id { get; set; }
        [ForeignKey("AspNetUsers")]
        public string users_id { get; set; }

        //create votetype enums
        public VoteType voteType { get; set; }
    }
}
