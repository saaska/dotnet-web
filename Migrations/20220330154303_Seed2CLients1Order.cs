using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dotnetweb.Migrations
{
    public partial class Seed2CLients1Order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "BirthDate", "Email", "Inn", "Name", "PhoneNumber" },
                values: new object[] { 2, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "nokia@bell-labs.com", "1234567890", "Юникс Мультиксович Линукс", "+14151234567" });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "BirthDate", "Email", "Inn", "Name", "PhoneNumber" },
                values: new object[] { 3, new DateTime(1854, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "sherlock@holmes.co.uk", "1234567890", "Шерлок Холмс", "+4402072243688" });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "ClientId", "CreatedOn", "Name", "Status" },
                values: new object[] { 1, 1, new DateTime(2022, 3, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Dragon Skin", 2 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
