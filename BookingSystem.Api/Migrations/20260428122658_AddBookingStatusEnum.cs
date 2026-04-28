using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE Bookings
                SET Status = CASE
                WHEN Status = 'Active' THEN '1'
                WHEN Status = 'Cancelled' THEN '2'
                WHEN Status = 'Completed' THEN '3'
                ELSE '1'
                END
        ");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Bookings",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
