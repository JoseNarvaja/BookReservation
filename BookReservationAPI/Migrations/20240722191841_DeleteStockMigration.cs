using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookReservationAPI.Migrations
{
    /// <inheritdoc />
    public partial class DeleteStockMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Books");

            migrationBuilder.AddColumn<int>(
                name: "CopyId",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Copies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Barcode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    BookId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Copies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Copies_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CopyId",
                table: "Reservations",
                column: "CopyId");

            migrationBuilder.CreateIndex(
                name: "IX_Copies_Barcode",
                table: "Copies",
                column: "Barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Copies_BookId",
                table: "Copies",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Copies_CopyId",
                table: "Reservations",
                column: "CopyId",
                principalTable: "Copies",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Copies_CopyId",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "Copies");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_CopyId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CopyId",
                table: "Reservations");

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
