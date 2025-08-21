using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankAccountServices.Migrations
{
    /// <inheritdoc />
    public partial class ajouterRefreshTokenUserForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<long>(
                name: "id_user",
                table: "ba_refresh_token",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ba_refresh_token_id_user",
                table: "ba_refresh_token",
                column: "id_user");

            migrationBuilder.AddForeignKey(
                name: "FK_ba_refresh_token_ba_user_id_user",
                table: "ba_refresh_token",
                column: "id_user",
                principalTable: "ba_user",
                principalColumn: "id_user",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ba_refresh_token_ba_user_id_user",
                table: "ba_refresh_token");

            migrationBuilder.DropIndex(
                name: "IX_ba_refresh_token_id_user",
                table: "ba_refresh_token");

            migrationBuilder.DropColumn(
                name: "id_user",
                table: "ba_refresh_token");

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
    }
}
