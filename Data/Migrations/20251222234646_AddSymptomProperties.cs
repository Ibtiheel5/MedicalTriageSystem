using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalTriageSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSymptomProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Doctors_DoctorId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Users_UserId",
                table: "Doctors");

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "TriageResults",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "TriageResults",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "TriageResults",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Symptoms",
                table: "TriageResults",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Severity",
                table: "Symptoms",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Symptoms",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Symptoms",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Symptoms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Symptoms",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AggravatingFactors",
                table: "Symptoms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Symptoms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Symptoms",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Frequency",
                table: "Symptoms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Intensity",
                table: "Symptoms",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Symptoms",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Symptoms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RelievingFactors",
                table: "Symptoms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Symptoms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Symptoms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Patients",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Patients",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Patients",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Patients",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Patients",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Patients",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Patients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BloodType",
                table: "Patients",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact",
                table: "Patients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmergencyPhone",
                table: "Patients",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "Notifications",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Notifications",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Specialty",
                table: "Doctors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Doctors",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Doctors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Doctors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Availability",
                table: "Doctors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "Doctors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Appointments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Appointments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Appointments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                table: "Appointments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "AppointmentDate",
                table: "Appointments",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Appointments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PatientId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    FileType = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Email",
                table: "Patients",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DoctorId",
                table: "Notifications",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_PatientId",
                table: "Notifications",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_LicenseNumber",
                table: "Doctors",
                column: "LicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_PatientId",
                table: "MedicalRecords",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Doctors_DoctorId",
                table: "Appointments",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Users_UserId",
                table: "Doctors",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Doctors_DoctorId",
                table: "Notifications",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Patients_PatientId",
                table: "Notifications",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Doctors_DoctorId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Users_UserId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Doctors_DoctorId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Patients_PatientId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "MedicalRecords");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Patients_Email",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_DoctorId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_PatientId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_LicenseNumber",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "TriageResults");

            migrationBuilder.DropColumn(
                name: "Symptoms",
                table: "TriageResults");

            migrationBuilder.DropColumn(
                name: "AggravatingFactors",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "Intensity",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "RelievingFactors",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "BloodType",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "EmergencyContact",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "EmergencyPhone",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "AppointmentDate",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "TriageResults",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "TriageResults",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "Severity",
                table: "Symptoms",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Symptoms",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Symptoms",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Symptoms",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Symptoms",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Symptoms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Patients",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Patients",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Patients",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Patients",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Patients",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Patients",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "Specialty",
                table: "Doctors",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Doctors",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Doctors",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Doctors",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Availability",
                table: "Doctors",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Appointments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Appointments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Appointments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                table: "Appointments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "Availability", "Email", "IsAvailable", "Name", "Phone", "Specialty", "UserId" },
                values: new object[,]
                {
                    { 1, "Lun-Ven: 9h-18h", "m.dubois@clinique.fr", true, "Dr. Martin Dubois", "01 23 45 67 89", "Médecine Générale", null },
                    { 2, "24/7 - Urgences", "s.laurent@clinique.fr", true, "Dr. Sophie Laurent", "01 98 76 54 32", "Urgentiste", null },
                    { 3, "Mar-Jeu: 10h-16h", "j.moreau@clinique.fr", false, "Dr. Jean Moreau", "01 12 34 56 78", "Cardiologie", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 23, 0, 13, 49, 480, DateTimeKind.Local).AddTicks(4576), "admin@triagemed.com", true, "$2a$11$2NoYO91n/mlSB9iEk8hT/OAjo5fXSLgGT8UBxvsOkL1thphvzmT2e", "Admin", "admin" },
                    { 2, new DateTime(2025, 12, 23, 0, 13, 49, 636, DateTimeKind.Local).AddTicks(4994), "docteur@clinique.fr", true, "$2a$11$2slcMsN2n2xJslmpNrt0AesdsnelkXLsiz5hrBjKigBvLDg5.ejfm", "Doctor", "docteur1" },
                    { 3, new DateTime(2025, 12, 23, 0, 13, 49, 797, DateTimeKind.Local).AddTicks(9117), "patient@example.com", true, "$2a$11$lpvAPvecjB/oA.0pBvZ5F.TKzCx05WwX5jm9IEBKVjAsTjL/Y1kxm", "Patient", "patient1" }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "Age", "CreatedAt", "Email", "Gender", "Name", "Phone", "UpdatedAt", "UserId" },
                values: new object[] { 1, 35, new DateTime(2025, 12, 23, 0, 13, 49, 797, DateTimeKind.Local).AddTicks(9745), "patient@example.com", "Homme", "Jean Dupont", "06 12 34 56 78", new DateTime(2025, 12, 23, 0, 13, 49, 797, DateTimeKind.Local).AddTicks(9746), 3 });

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Doctors_DoctorId",
                table: "Appointments",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patients_PatientId",
                table: "Appointments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Users_UserId",
                table: "Doctors",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
