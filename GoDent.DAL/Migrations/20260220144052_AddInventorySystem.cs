using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoDent.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddInventorySystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfBirth",
                table: "Patients",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "Appointments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    MinQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tools_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dental Surgery Department", "Surgery" },
                    { 2, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Radiology Department", "Radiology" },
                    { 3, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sterilization Department", "Sterilization" },
                    { 4, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Orthodontics Department", "Orthodontics" },
                    { 5, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Endodontics Department", "Endodontics" },
                    { 6, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fixed Prothodontics Department", "Fixed Prothodontics" },
                    { 7, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Removable Prothodontics Department", "Removable" },
                    { 8, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Conservative Dentistry Department", "Conservative" },
                    { 9, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pediatric Dentistry Department", "Pedodontics" },
                    { 10, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Periodontics Department", "Periodontics" },
                    { 11, new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Other Departments", "Other" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tools_DepartmentId",
                table: "Tools",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tools");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Patients",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "Appointments",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
