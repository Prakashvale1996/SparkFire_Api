using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrackersStore.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 17, 16, 22, 13, 758, DateTimeKind.Utc).AddTicks(3207), new DateTime(2025, 12, 17, 16, 22, 13, 758, DateTimeKind.Utc).AddTicks(3209) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 17, 16, 22, 13, 758, DateTimeKind.Utc).AddTicks(3225), new DateTime(2025, 12, 17, 16, 22, 13, 758, DateTimeKind.Utc).AddTicks(3225) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 17, 15, 2, 0, 520, DateTimeKind.Utc).AddTicks(6996), new DateTime(2025, 12, 17, 15, 2, 0, 520, DateTimeKind.Utc).AddTicks(6998) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 17, 15, 2, 0, 520, DateTimeKind.Utc).AddTicks(7009), new DateTime(2025, 12, 17, 15, 2, 0, 520, DateTimeKind.Utc).AddTicks(7010) });
        }
    }
}
