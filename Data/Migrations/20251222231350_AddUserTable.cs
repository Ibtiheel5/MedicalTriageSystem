using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalTriageSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patients_UserId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 23, 0, 13, 49, 797, DateTimeKind.Local).AddTicks(9745), new DateTime(2025, 12, 23, 0, 13, 49, 797, DateTimeKind.Local).AddTicks(9746) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 23, 0, 13, 49, 480, DateTimeKind.Local).AddTicks(4576), "$2a$11$2NoYO91n/mlSB9iEk8hT/OAjo5fXSLgGT8UBxvsOkL1thphvzmT2e" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 23, 0, 13, 49, 636, DateTimeKind.Local).AddTicks(4994), "$2a$11$2slcMsN2n2xJslmpNrt0AesdsnelkXLsiz5hrBjKigBvLDg5.ejfm" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 23, 0, 13, 49, 797, DateTimeKind.Local).AddTicks(9117), "$2a$11$lpvAPvecjB/oA.0pBvZ5F.TKzCx05WwX5jm9IEBKVjAsTjL/Y1kxm" });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_UserId",
                table: "Patients",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patients_UserId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 22, 14, 29, 16, 632, DateTimeKind.Local).AddTicks(1975), new DateTime(2025, 12, 22, 14, 29, 16, 632, DateTimeKind.Local).AddTicks(1977) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 22, 14, 29, 16, 309, DateTimeKind.Local).AddTicks(1231), "$2a$11$/adnyukiWuqZRLOuhcTLkOF4Lv3I.X0VzPxlTwK/rbVKrapWEDV4O" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 22, 14, 29, 16, 471, DateTimeKind.Local).AddTicks(583), "$2a$11$dNjJvEwwRA85rBElUQsvmu/cQfFUFaRmzdM0Vj3dOAyd5rpX8lxge" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 22, 14, 29, 16, 632, DateTimeKind.Local).AddTicks(1070), "$2a$11$wL1uGh5Q/YlI4w4gRr4/KeOj0xLAuugdIRdSzh.r/gEuS/nKE6eR6" });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_UserId",
                table: "Patients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors",
                column: "UserId");
        }
    }
}
