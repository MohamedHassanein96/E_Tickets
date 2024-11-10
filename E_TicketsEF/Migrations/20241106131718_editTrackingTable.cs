using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_TicketsEF.Migrations
{
    /// <inheritdoc />
    public partial class editTrackingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "TrackingSales",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "TrackingSales");
        }
    }
}
