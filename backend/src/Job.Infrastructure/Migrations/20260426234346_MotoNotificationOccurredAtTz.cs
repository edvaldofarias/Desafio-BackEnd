using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MotoNotificationOccurredAtTz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Manager",
                keyColumn: "Id",
                keyValue: new Guid("be555cd6-8f93-4497-b455-de3f58c486ab"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OccurredAt",
                table: "MotoNotification",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.InsertData(
                table: "Manager",
                columns: new[] { "Id", "Created", "Email", "Password", "Updated" },
                values: new object[] { new Guid("5bcd4a18-ddfd-4a68-a291-aa17a49966a8"), new DateTime(2026, 4, 26, 20, 43, 46, 535, DateTimeKind.Local).AddTicks(5800), "job@job.com", "$2a$12$gcHF.JBlb21bKBild2YINu1uUuHVkaoXw5UlcxQag9LbzLh0tVn6i", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Manager",
                keyColumn: "Id",
                keyValue: new Guid("5bcd4a18-ddfd-4a68-a291-aa17a49966a8"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OccurredAt",
                table: "MotoNotification",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.InsertData(
                table: "Manager",
                columns: new[] { "Id", "Created", "Email", "Password", "Updated" },
                values: new object[] { new Guid("be555cd6-8f93-4497-b455-de3f58c486ab"), new DateTime(2026, 4, 21, 18, 50, 2, 793, DateTimeKind.Local).AddTicks(3780), "job@job.com", "$2a$12$bQHB7upL4vu7jM5weENUEuGiOpigBwFuwXx4dYri1C2ib4A/6Y2Gu", null });
        }
    }
}
