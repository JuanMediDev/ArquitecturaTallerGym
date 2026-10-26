using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Gimnasio.Datos.Migraciones
{
    /// <inheritdoc />
    public partial class InstructoresDemostracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Instructores",
                columns: new[] { "Id", "Especialidad", "Nombre" },
                values: new object[,]
                {
                    { 1, "Fuerza y acondicionamiento", "Valeria Ríos" },
                    { 2, "Ciclismo indoor", "Mateo Cruz" },
                    { 3, "Movilidad y estiramiento", "Camila Torres" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Instructores",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Instructores",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Instructores",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
