﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NetworkingPlatform.Models
{
    public class CommentHistory
    {
        [Key]
        public int ID { get; set; }
        public string? Content { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("PostComments")]
        public int comment_id { get; set; }

        [ForeignKey("AspNetUsers")]
        public string? users_id { get; set; }

    }
}
