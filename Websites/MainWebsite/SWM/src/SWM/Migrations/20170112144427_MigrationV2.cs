using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SWM.Migrations
{
    public partial class MigrationV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FarmLocationId",
                table: "CropDatas");

            migrationBuilder.AddColumn<int>(
                name: "UserLocationToMachineId",
                table: "CropDatas",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserLocationToMachineId",
                table: "CropDatas");

            migrationBuilder.AddColumn<int>(
                name: "FarmLocationId",
                table: "CropDatas",
                nullable: false,
                defaultValue: 0);
        }
    }
}
