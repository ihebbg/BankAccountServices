using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankAccountServices.Migrations
{
    /// <inheritdoc />
    public partial class ajouterRefreshTokenUserForeign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserIdUser",
                table: "ba_refresh_token",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ba_refresh_token_UserIdUser",
                table: "ba_refresh_token",
                column: "UserIdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_ba_refresh_token_ba_user_UserIdUser",
                table: "ba_refresh_token",
                column: "UserIdUser",
                principalTable: "ba_user",
                principalColumn: "id_user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ba_refresh_token_ba_user_UserIdUser",
                table: "ba_refresh_token");

            migrationBuilder.DropIndex(
                name: "IX_ba_refresh_token_UserIdUser",
                table: "ba_refresh_token");

            migrationBuilder.DropColumn(
                name: "UserIdUser",
                table: "ba_refresh_token");
        }
    }
}
