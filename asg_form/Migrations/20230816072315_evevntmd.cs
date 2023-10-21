using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asg_form.Migrations
{
    /// <inheritdoc />
    public partial class evevntmd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "events_rule_uri",
                table: "F_events",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "events_rule_uri",
                table: "F_events");
        }
    }
}
