using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dotnetweb.Migrations
{
    public partial class Seed1Client : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "BirthDate", "Email", "Inn", "Name", "PhoneNumber" },
                values: new object[] { 1, new DateTime(1970, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "140123456789", "Поликарп Петров", "+79245551122" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
