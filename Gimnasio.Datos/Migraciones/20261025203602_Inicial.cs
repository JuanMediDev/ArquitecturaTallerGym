using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gimnasio.Datos.Migraciones
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Documento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Instructores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Especialidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanesMembresia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DuracionDias = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Beneficios = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanesMembresia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InstructorId = table.Column<int>(type: "int", nullable: false),
                    FechaHoraInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaHoraFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CupoMaximo = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clases_Instructores_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Membresias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membresias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Membresias_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Membresias_PlanesMembresia_PlanId",
                        column: x => x.PlanId,
                        principalTable: "PlanesMembresia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Asistencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    ClaseId = table.Column<int>(type: "int", nullable: false),
                    FechaHoraRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asistencias_Clases_ClaseId",
                        column: x => x.ClaseId,
                        principalTable: "Clases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asistencias_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    ClaseId = table.Column<int>(type: "int", nullable: false),
                    FechaReserva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservas_Clases_ClaseId",
                        column: x => x.ClaseId,
                        principalTable: "Clases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MembresiaId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagos_Membresias_MembresiaId",
                        column: x => x.MembresiaId,
                        principalTable: "Membresias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_ClaseId",
                table: "Asistencias",
                column: "ClaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_ClienteId",
                table: "Asistencias",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Clases_InstructorId",
                table: "Clases",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Membresias_ClienteId",
                table: "Membresias",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Membresias_PlanId",
                table: "Membresias",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_MembresiaId",
                table: "Pagos",
                column: "MembresiaId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_ClaseId",
                table: "Reservas",
                column: "ClaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_ClienteId_ClaseId",
                table: "Reservas",
                columns: new[] { "ClienteId", "ClaseId" },
                unique: true,
                filter: "[Estado] = N'Activa'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asistencias");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Membresias");

            migrationBuilder.DropTable(
                name: "Clases");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "PlanesMembresia");

            migrationBuilder.DropTable(
                name: "Instructores");
        }
    }
}
