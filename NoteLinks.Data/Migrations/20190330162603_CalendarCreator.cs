using Microsoft.EntityFrameworkCore.Migrations;

namespace NoteLinks.Data.Migrations
{
    public partial class CalendarCreator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Calendars",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Calendars_CreatorId",
                table: "Calendars",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Calendars_AspNetUsers_CreatorId",
                table: "Calendars",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calendars_AspNetUsers_CreatorId",
                table: "Calendars");

            migrationBuilder.DropIndex(
                name: "IX_Calendars_CreatorId",
                table: "Calendars");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Calendars");
        }
    }
}
