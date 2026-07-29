using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilleteraCriptomonedasBackend.Migrations
{
    /// <inheritdoc />
    public partial class MigracionConNombreUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NombreUsuario",
                table: "Clientes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreUsuario",
                table: "Clientes");
        }
    }
}
