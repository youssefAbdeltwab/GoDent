using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoDent.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentDebtToPatientAddWhatsUPMessagingForRemindPatients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurrentDebt",
                table: "Patients",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentDebt",
                table: "Patients");
        }
    }
}
