using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImportDataCv.Server.Migrations
{
    /// <inheritdoc />
    public partial class CreateZaloMessagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ZaloMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OaId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    uId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    uName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZaloMessages", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ZaloMessages");
        }
    }
}
