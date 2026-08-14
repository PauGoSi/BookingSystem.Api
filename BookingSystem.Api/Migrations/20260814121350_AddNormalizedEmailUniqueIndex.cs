using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNormalizedEmailUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Fail safely if an existing email cannot fit in the new column.
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM Users
                    WHERE LEN(LTRIM(RTRIM(Email))) > 254
                )
                BEGIN
                    THROW 50001, 'One or more existing email addresses exceed 254 characters.', 1;
                END
                """
            );

            // Fail safely if existing users would become duplicates after normalization.
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT UPPER(LTRIM(RTRIM(Email)))
                    FROM Users
                GROUP BY UPPER(LTRIM(RTRIM(Email)))
                HAVING COUNT(*) > 1
                )
                BEGIN
                THROW 50002, 'Duplicate email addresses exist after normalization.', 1;
                END
                """
            );

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(254)",
                maxLength: 254,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Users",
                type: "nvarchar(254)",
                maxLength: 254,
                nullable: false,
                defaultValue: "");

            // Backfill existing users before creating the unique index.
            migrationBuilder.Sql(
                """
                UPDATE Users
                SET
                Email = LTRIM(RTRIM(Email)),
                NormalizedEmail = UPPER(LTRIM(RTRIM(Email)));
                """
            );

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedEmail",
                table: "Users",
                column: "NormalizedEmail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_NormalizedEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(254)",
                oldMaxLength: 254);
        }
    }
}
