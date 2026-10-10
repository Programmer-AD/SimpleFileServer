using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleFileServer.Infrastructure.Migrations;

/// <inheritdoc />
public partial class MakeLocationOptional : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "file_location",
            table: "T_DomainFiles",
            type: "TEXT",
            maxLength: 1000,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldMaxLength: 1000);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "file_location",
            table: "T_DomainFiles",
            type: "TEXT",
            maxLength: 1000,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "TEXT",
            oldMaxLength: 1000,
            oldNullable: true);
    }
}
