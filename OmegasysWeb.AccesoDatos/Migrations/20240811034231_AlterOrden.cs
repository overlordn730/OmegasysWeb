using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OmegasysWeb.AccesoDatos.Migrations
{
    public partial class AlterOrden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "Ordenes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Ordenes");
        }
    }
}
