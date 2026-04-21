using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlignSwaggerContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Manager",
                keyColumn: "Id",
                keyValue: new Guid("af2effff-71cc-4069-a679-dde8c44988bc"));

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "Rental",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "Fine",
                table: "Rental",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DailyValue",
                table: "Rental",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateReturn",
                table: "Rental",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Identifier",
                table: "Rental",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Identifier",
                table: "Motoboy",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Identifier",
                table: "Moto",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Manager",
                columns: new[] { "Id", "Created", "Email", "Password", "Updated" },
                values: new object[] { new Guid("be555cd6-8f93-4497-b455-de3f58c486ab"), new DateTime(2026, 4, 21, 18, 50, 2, 793, DateTimeKind.Local).AddTicks(3780), "job@job.com", "$2a$12$bQHB7upL4vu7jM5weENUEuGiOpigBwFuwXx4dYri1C2ib4A/6Y2Gu", null });

            migrationBuilder.CreateIndex(
                name: "IX_Rental_Identifier",
                table: "Rental",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rental_IdMoto",
                table: "Rental",
                column: "IdMoto");

            migrationBuilder.CreateIndex(
                name: "IX_Motoboy_Identifier",
                table: "Motoboy",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Moto_Identifier",
                table: "Moto",
                column: "Identifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rental_Identifier",
                table: "Rental");

            migrationBuilder.DropIndex(
                name: "IX_Rental_IdMoto",
                table: "Rental");

            migrationBuilder.DropIndex(
                name: "IX_Motoboy_Identifier",
                table: "Motoboy");

            migrationBuilder.DropIndex(
                name: "IX_Moto_Identifier",
                table: "Moto");

            migrationBuilder.DeleteData(
                table: "Manager",
                keyColumn: "Id",
                keyValue: new Guid("be555cd6-8f93-4497-b455-de3f58c486ab"));

            migrationBuilder.DropColumn(
                name: "DailyValue",
                table: "Rental");

            migrationBuilder.DropColumn(
                name: "DateReturn",
                table: "Rental");

            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "Rental");

            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "Motoboy");

            migrationBuilder.DropColumn(
                name: "Identifier",
                table: "Moto");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "Rental",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Fine",
                table: "Rental",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Manager",
                columns: new[] { "Id", "Created", "Email", "Password", "Updated" },
                values: new object[] { new Guid("af2effff-71cc-4069-a679-dde8c44988bc"), new DateTime(2026, 4, 21, 18, 13, 35, 886, DateTimeKind.Local).AddTicks(8590), "job@job.com", "$2a$12$asaPyf07mop4rHaXgbqQp.24VJtsDu.8QCPanxFRrz.raKQNCbyiO", null });
        }
    }
}
