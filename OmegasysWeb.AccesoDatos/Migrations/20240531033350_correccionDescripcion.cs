using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmegasysWeb.AccesoDatos.Migrations
{
    public partial class correccionDescripcion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descipcion",
                table: "Productos",
                newName: "Descripcion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "Productos",
                newName: "Descipcion");
        }
    }
}
