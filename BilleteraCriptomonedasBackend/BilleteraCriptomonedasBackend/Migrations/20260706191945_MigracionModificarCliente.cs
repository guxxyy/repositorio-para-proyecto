using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilleteraCriptomonedasBackend.Migrations
{
    /// <inheritdoc />
    public partial class MigracionModificarCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "Transacciones",
                newName: "NombreCliente");

            migrationBuilder.RenameColumn(
                name: "NombreUsuario",
                table: "Clientes",
                newName: "NombreCliente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NombreCliente",
                table: "Transacciones",
                newName: "ClienteId");

            migrationBuilder.RenameColumn(
                name: "NombreCliente",
                table: "Clientes",
                newName: "NombreUsuario");
        }
    }
}
