using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asg_form.Migrations
{
    /// <inheritdoc />
    public partial class champ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "F_Champion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    formId = table.Column<long>(type: "bigint", nullable: false),
                    eventsId = table.Column<int>(type: "int", nullable: false),
                    msg = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F_Champion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_F_Champion_F_events_eventsId",
                        column: x => x.eventsId,
                        principalTable: "F_events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_F_Champion_F_form_formId",
                        column: x => x.formId,
                        principalTable: "F_form",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_F_Champion_eventsId",
                table: "F_Champion",
                column: "eventsId");

            migrationBuilder.CreateIndex(
                name: "IX_F_Champion_formId",
                table: "F_Champion",
                column: "formId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "F_Champion");
        }
    }
}
