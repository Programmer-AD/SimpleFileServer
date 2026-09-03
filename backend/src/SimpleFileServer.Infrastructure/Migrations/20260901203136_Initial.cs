using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleFileServer.Infrastructure.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "T_DomainFiles",
            columns: table => new
            {
                id = table.Column<Guid>(type: "TEXT", nullable: false),
                name = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                size = table.Column<long>(type: "INTEGER", nullable: false),
                created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                file_location = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_T_DomainFiles", x => x.id));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "T_DomainFiles");
    }
}
