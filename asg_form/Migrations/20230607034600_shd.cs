using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asg_form.Migrations
{
    /// <inheritdoc />
    public partial class shd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "F_game",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    team1_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    team1_piaoshu = table.Column<int>(type: "int", nullable: false),
                    team2_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    team2_piaoshu = table.Column<int>(type: "int", nullable: false),
                    opentime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    closetime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F_game", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "F_achlog",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    teamid = table.Column<long>(type: "bigint", nullable: false),
                    chickteam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    win = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F_achlog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_F_achlog_F_game_teamid",
                        column: x => x.teamid,
                        principalTable: "F_game",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_F_achlog_teamid",
                table: "F_achlog",
                column: "teamid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "F_achlog");

            migrationBuilder.DropTable(
                name: "F_game");
        }
    }
}
