using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiMySQL.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingLineGripColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Grip",
                table: "TrainingLines",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Grip",
                table: "TrainingLines");
        }
    }
}
