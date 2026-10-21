using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bilionn_backend.Migrations
{
    /// <inheritdoc />
    public partial class UseHashedPasswordColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "tbl_users");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "tbl_users",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "tbl_users");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "tbl_users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
