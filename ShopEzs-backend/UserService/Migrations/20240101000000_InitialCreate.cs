using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id       = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                Name     = table.Column<string>(maxLength: 100, nullable: false),
                Email    = table.Column<string>(maxLength: 200, nullable: false),
                Password = table.Column<string>(nullable: false),
                Role     = table.Column<string>(nullable: false, defaultValue: "User")
            },
            constraints: table => table.PrimaryKey("PK_Users", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Users");
    }
}
