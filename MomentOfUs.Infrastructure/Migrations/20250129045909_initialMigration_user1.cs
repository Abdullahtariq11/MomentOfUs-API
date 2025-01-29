using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MomentOfUs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initialMigration_user1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a29f7b85-9f5f-4b0e-9497-9c6f91b8b1c4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "395db041-6ef7-4927-b528-e179801f9ef2", "Sameen11.", "6e0fe864-3e49-4144-93af-4e1728297488" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a29f7b85-9f5f-4b0e-9497-9c6f91b8b1c4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ce03c6f9-1f21-4b76-acb3-7560bfb18535", "AQAAAAIAAYagAAAAENhRuarjcfF6CEMaZBu9tQLNcdENMR+XUPrNiVaZK+8xuiUBOvUrh0kaoI3xeljaEg==", "c4d3b80a-8856-4ae0-b7af-91e2d87f9af7" });
        }
    }
}
