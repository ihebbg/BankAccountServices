using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankAccountServices.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ba_customer",
                columns: table => new
                {
                    id_customer = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ba_customer", x => x.id_customer);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ba_bank_account",
                columns: table => new
                {
                    id_bank_account = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    balance = table.Column<double>(type: "double", nullable: false),
                    created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    currency = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false),
                    id_customer = table.Column<long>(type: "bigint", nullable: false),
                    account_type = table.Column<string>(type: "varchar(13)", maxLength: 13, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    overdraft = table.Column<double>(type: "double", nullable: true),
                    interestrate = table.Column<double>(type: "double", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ba_bank_account", x => x.id_bank_account);
                    table.ForeignKey(
                        name: "FK_ba_bank_account_ba_customer_id_customer",
                        column: x => x.id_customer,
                        principalTable: "ba_customer",
                        principalColumn: "id_customer",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ba_acount_operation",
                columns: table => new
                {
                    id_account_operation = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    amount = table.Column<double>(type: "double", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    id_bank_account = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ba_acount_operation", x => x.id_account_operation);
                    table.ForeignKey(
                        name: "FK_ba_acount_operation_ba_bank_account_id_bank_account",
                        column: x => x.id_bank_account,
                        principalTable: "ba_bank_account",
                        principalColumn: "id_bank_account",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ba_acount_operation_id_bank_account",
                table: "ba_acount_operation",
                column: "id_bank_account");

            migrationBuilder.CreateIndex(
                name: "IX_ba_bank_account_id_customer",
                table: "ba_bank_account",
                column: "id_customer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ba_acount_operation");

            migrationBuilder.DropTable(
                name: "ba_bank_account");

            migrationBuilder.DropTable(
                name: "ba_customer");
        }
    }
}
