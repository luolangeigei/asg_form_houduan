using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asg_form.Migrations
{
    /// <inheritdoc />
    public partial class newstime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "closetime",
                table: "F_game");

            migrationBuilder.AddColumn<DateTime>(
                name: "time",
                table: "F_news",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "bilibiliuri",
                table: "F_game",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "commentary",
                table: "F_game",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "referee",
                table: "F_game",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "winteam",
                table: "F_game",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "chinaname",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "officium",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "time",
                table: "F_news");

            migrationBuilder.DropColumn(
                name: "bilibiliuri",
                table: "F_game");

            migrationBuilder.DropColumn(
                name: "commentary",
                table: "F_game");

            migrationBuilder.DropColumn(
                name: "referee",
                table: "F_game");

            migrationBuilder.DropColumn(
                name: "winteam",
                table: "F_game");

            migrationBuilder.DropColumn(
                name: "chinaname",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "officium",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "closetime",
                table: "F_game",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
