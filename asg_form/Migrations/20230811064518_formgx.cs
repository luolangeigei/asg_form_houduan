using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asg_form.Migrations
{
    /// <inheritdoc />
    public partial class formgx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "winteam",
                table: "F_game",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "belong",
                table: "F_game",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "eventsId",
                table: "F_form",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "win",
                table: "F_achlog",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateTable(
                name: "F_events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_over = table.Column<bool>(type: "bit", nullable: true),
                    opentime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F_events", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_F_form_eventsId",
                table: "F_form",
                column: "eventsId");

            migrationBuilder.AddForeignKey(
                name: "FK_F_form_F_events_eventsId",
                table: "F_form",
                column: "eventsId",
                principalTable: "F_events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_F_form_F_events_eventsId",
                table: "F_form");

            migrationBuilder.DropTable(
                name: "F_events");

            migrationBuilder.DropIndex(
                name: "IX_F_form_eventsId",
                table: "F_form");

            migrationBuilder.DropColumn(
                name: "belong",
                table: "F_game");

            migrationBuilder.DropColumn(
                name: "eventsId",
                table: "F_form");

            migrationBuilder.AlterColumn<string>(
                name: "winteam",
                table: "F_game",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "win",
                table: "F_achlog",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }
    }
}
