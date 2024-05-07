using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace NetworkingPlatform.Migrations
{
    public partial class AddReplyVotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReplyVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reply_id = table.Column<int>(type: "int", nullable: false),
                    users_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    voteType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplyVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReplyVotes_AspNetUsers_users_id",
                        column: x => x.users_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReplyVotes_Reply_reply_id",
                        column: x => x.reply_id,
                        principalTable: "Reply",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReplyVotes_reply_id",
                table: "ReplyVotes",
                column: "reply_id");

            migrationBuilder.CreateIndex(
                name: "IX_ReplyVotes_users_id",
                table: "ReplyVotes",
                column: "users_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReplyVotes");
        }
    }
}
