using Microsoft.EntityFrameworkCore.Migrations;

namespace NetworkingPlatform.Migrations
{
    public partial class AddVotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_id = table.Column<int>(nullable: false),
                    users_id = table.Column<string>(maxLength: 450, nullable: true), // Ensure the length matches the AspNetUsers.Id column
                    voteType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votes_Posts_post_id",
                        column: x => x.post_id,
                        principalTable: "Posts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votes_AspNetUsers_users_id",
                        column: x => x.users_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict); // Change the behavior to restrict to avoid column length mismatch
                });

            migrationBuilder.CreateIndex(
                name: "IX_Votes_post_id",
                table: "Votes",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_users_id",
                table: "Votes",
                column: "users_id");
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Votes");
        }
    }
}
