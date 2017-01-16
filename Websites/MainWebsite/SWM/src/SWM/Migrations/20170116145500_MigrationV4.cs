using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SWM.Migrations
{
    public partial class MigrationV4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PinId",
                table: "UserLocations");

            migrationBuilder.DropTable(
                name: "PinNumbers");

            migrationBuilder.AddColumn<int>(
                name: "PinNo",
                table: "UserLocations",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PinNo",
                table: "UserLocations");

            migrationBuilder.CreateTable(
                name: "PinNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pin = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinNumbers", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "PinId",
                table: "UserLocations",
                nullable: false,
                defaultValue: 0);
        }
    }
}
