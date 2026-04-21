using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Job.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexesAndMotoNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Manager",
                keyColumn: "Id",
                keyValue: new Guid("0de94dd6-d8a6-45d9-a40f-2e4907fa137e"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Motoboy",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Cnpj",
                table: "Motoboy",
                type: "character varying(14)",
                maxLength: 14,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CnhImage",
                table: "Motoboy",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cnh",
                table: "Motoboy",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Plate",
                table: "Moto",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "Moto",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "MotoNotification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MotoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Plate = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotoNotification", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Manager",
                columns: new[] { "Id", "Created", "Email", "Password", "Updated" },
                values: new object[] { new Guid("af2effff-71cc-4069-a679-dde8c44988bc"), new DateTime(2026, 4, 21, 18, 13, 35, 886, DateTimeKind.Local).AddTicks(8590), "job@job.com", "$2a$12$asaPyf07mop4rHaXgbqQp.24VJtsDu.8QCPanxFRrz.raKQNCbyiO", null });

            migrationBuilder.CreateIndex(
                name: "IX_Motoboy_Cnh",
                table: "Motoboy",
                column: "Cnh",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Motoboy_Cnpj",
                table: "Motoboy",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Moto_Plate",
                table: "Moto",
                column: "Plate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MotoNotification_MotoId",
                table: "MotoNotification",
                column: "MotoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MotoNotification");

            migrationBuilder.DropIndex(
                name: "IX_Motoboy_Cnh",
                table: "Motoboy");

            migrationBuilder.DropIndex(
                name: "IX_Motoboy_Cnpj",
                table: "Motoboy");

            migrationBuilder.DropIndex(
                name: "IX_Moto_Plate",
                table: "Moto");

            migrationBuilder.DeleteData(
                table: "Manager",
                keyColumn: "Id",
                keyValue: new Guid("af2effff-71cc-4069-a679-dde8c44988bc"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Motoboy",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Cnpj",
                table: "Motoboy",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(14)",
                oldMaxLength: 14);

            migrationBuilder.AlterColumn<string>(
                name: "CnhImage",
                table: "Motoboy",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cnh",
                table: "Motoboy",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "Plate",
                table: "Moto",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "Moto",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.InsertData(
                table: "Manager",
                columns: new[] { "Id", "Created", "Email", "Password", "Updated" },
                values: new object[] { new Guid("0de94dd6-d8a6-45d9-a40f-2e4907fa137e"), new DateTime(2024, 9, 30, 12, 4, 8, 46, DateTimeKind.Local).AddTicks(7143), "job@job.com", "$2a$12$8atmAaWnqBeSigICtB813edOCp8PFoprZLp/TNQNwBeLAYYwVrfn.", null });
        }
    }
}
