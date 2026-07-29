using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilleteraCriptomonedasBackend.Migrations
{
    /// <inheritdoc />
    public partial class RelacionUnoAUnoUsuarioBilletera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Billetera",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Bitcoin = table.Column<decimal>(type: "TEXT", nullable: false),
                    Ethereum = table.Column<decimal>(type: "TEXT", nullable: false),
                    USDT = table.Column<decimal>(type: "TEXT", nullable: false),
                    IdCliente = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Billetera", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Billetera_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Billetera_IdCliente",
                table: "Billetera",
                column: "IdCliente",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Billetera");
        }
    }
}
