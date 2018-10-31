using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NoteLinks.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calendars",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CalendarId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Text = table.Column<string>(nullable: true),
                    FromDate = table.Column<DateTime>(nullable: false),
                    ToDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_Calendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "Calendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Calendars",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { 1, null, "Дни рождения родственников" });

            migrationBuilder.InsertData(
                table: "Calendars",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { 2, null, "Отпуска сотрудников" });

            migrationBuilder.InsertData(
                table: "Calendars",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[] { 3, null, "Поездка в Испанию" });

            migrationBuilder.InsertData(
                table: "Notes",
                columns: new[] { "Id", "CalendarId", "FromDate", "Name", "Text", "ToDate" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2019, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Артём", "Хочет Lego", null },
                    { 2, 1, new DateTime(2018, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Марина", "Пригласить Ивановых", null },
                    { 3, 1, new DateTime(2019, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Оксана Ивановна", "Уехать в командировку", null },
                    { 4, 2, new DateTime(2019, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Серёга", "Тайланд", new DateTime(2019, 3, 24, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 2, new DateTime(2019, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Рустем", "Хочет перенести на апрель", new DateTime(2019, 3, 19, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, 3, new DateTime(2019, 5, 29, 17, 40, 0, 0, DateTimeKind.Unspecified), "Самолет Сургут-Москва", "Взять книгу", new DateTime(2019, 5, 29, 18, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 7, 3, new DateTime(2019, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Хостел в Москве", "Заезд после 15:00", new DateTime(2019, 6, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, 3, new DateTime(2019, 6, 3, 9, 20, 0, 0, DateTimeKind.Unspecified), "Самолет Москва-Барселона", null, new DateTime(2019, 6, 3, 11, 5, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notes_CalendarId",
                table: "Notes",
                column: "CalendarId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "Calendars");
        }
    }
}
