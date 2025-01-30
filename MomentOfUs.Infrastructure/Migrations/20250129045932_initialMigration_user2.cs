using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MomentOfUs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initialMigration_user2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a29f7b85-9f5f-4b0e-9497-9c6f91b8b1c4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImageUrl", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "a29f7b85-9f5f-4b0e-9497-9c6f91b8b1c4", 0, "395db041-6ef7-4927-b528-e179801f9ef2", "abdullahtariq096@gmail.com", true, "Abdullah", "Tariq", false, null, "ABDULLAHTARIQ096@GMAIL.COM", "ABDULLAHT", "Sameen11.", null, false, null, "6e0fe864-3e49-4144-93af-4e1728297488", false, "abdullahT" });
        }
    }
}
