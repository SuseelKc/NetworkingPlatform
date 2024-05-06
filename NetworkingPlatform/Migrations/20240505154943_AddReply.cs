using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace NetworkingPlatform.Migrations
{
    public partial class AddReply : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Replies",
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
                    table.PrimaryKey("PK_Replies", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Replies_AspNetUsers_users_id",
                        column: x => x.users_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Replies_PostComments_comment_id",
                        column: x => x.comment_id,
                        principalTable: "PostComments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Replies_comment_id",
                table: "Replies",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_users_id",
                table: "Replies",
                column: "users_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Replies");
        }
    }
}
