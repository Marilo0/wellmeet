using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wellmeet.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxParticipantsToActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxParticipants",
                table: "Activities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxParticipants",
                table: "Activities");
        }
    }
}
