using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookReservationAPI.Migrations
{
    /// <inheritdoc />
    public partial class isDeletedCopyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Copies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Copies");
        }
    }
}
