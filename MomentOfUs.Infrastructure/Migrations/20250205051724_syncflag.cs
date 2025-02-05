using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MomentOfUs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class syncflag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSynced",
                table: "Journals",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSynced",
                table: "Journals");
        }
    }
}
