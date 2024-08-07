﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetworkingPlatform.Models;

namespace NetworkingPlatform.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {


        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<Votes> Votes { get; set; }
        public DbSet<PostComments> PostComments { get; set; }
        public DbSet<Reply> Reply { get; set; }
        public DbSet<ReplyVotes> ReplyVote { get; set; }
        public DbSet<CommentVotes> CommentVotes { get; set; }

        public DbSet<PostHistory> PostHistory { get; set; }

        public DbSet<CommentHistory> CommentHistory { get; set; }

    }
}
