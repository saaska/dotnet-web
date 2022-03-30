using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace dotnetweb.Migrations
{
    public partial class Seed3Orders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "Name" },
                values: new object[] { new DateTime(2022, 3, 30, 17, 53, 0, 0, DateTimeKind.Unspecified), "Драконьи шкуры" });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "ClientId", "CreatedOn", "Name", "Status" },
                values: new object[,]
                {
                    { 2, 1, new DateTime(2022, 3, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Автозапчасти", 1 },
                    { 3, 2, new DateTime(2021, 2, 1, 10, 5, 34, 0, DateTimeKind.Unspecified), "8\" Inch Floppy Disks", 0 },
                    { 4, 2, new DateTime(2019, 12, 31, 18, 30, 0, 0, DateTimeKind.Unspecified), "Bell Labs Technical Reports 1970-1974", 0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "Name" },
                values: new object[] { new DateTime(2022, 3, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Dragon Skin" });
        }
    }
}
