using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SWM.Migrations
{
    public partial class AdditionModification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FarmLocations",
                table: "FarmLocations");

            migrationBuilder.CreateTable(
                name: "MachineIds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    IsAssigned = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineIds", x => x.Id);
                });

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FarmLocations",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "FarmLocations",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "FarmLocations",
                nullable: false)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FarmLocations",
                table: "FarmLocations",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FarmLocations",
                table: "FarmLocations");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "FarmLocations");

            migrationBuilder.DropTable(
                name: "MachineIds");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "FarmLocations",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FarmLocations",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FarmLocations",
                table: "FarmLocations",
                columns: new[] { "UserId", "Name" });
        }
    }
}
