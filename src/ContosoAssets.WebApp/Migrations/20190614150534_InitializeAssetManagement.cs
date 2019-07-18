using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContosoAssets.WebApp.Migrations
{
    public partial class InitializeAssetManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Assets",
                table => new
                {
                    Id = table.Column<string>(),
                    CustomerName = table.Column<string>(nullable: true),
                    LastUpdateTime = table.Column<DateTimeOffset>(),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<int>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Assets");
        }
    }
}
