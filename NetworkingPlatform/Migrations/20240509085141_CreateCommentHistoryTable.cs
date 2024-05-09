using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace NetworkingPlatform.Migrations
{
    public partial class CreateCommentHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommentHistory",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    comment_id = table.Column<int>(type: "int", nullable: false),
                    users_id = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentHistory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CommentHistory_AspNetUsers_users_id",
                        column: x => x.users_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentHistory_PostComments_comment_id",
                        column: x => x.comment_id,
                        principalTable: "PostComments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentHistory_comment_id",
                table: "CommentHistory",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_CommentHistory_users_id",
                table: "CommentHistory",
                column: "users_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentHistory");
        }
    }
}
