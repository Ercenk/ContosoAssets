using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContosoAssets.SolutionManagement.Migrations.CustomerManagementDb
{
    public partial class InitializeCustomerManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CustomerName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skus",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    MonthlyCost = table.Column<double>(nullable: false),
                    MonthlyLimit = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SalesChannel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    CustomerName = table.Column<string>(nullable: true),
                    CustomerRegion = table.Column<int>(nullable: true),
                    ExternalUserName = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerUsers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerSubscriptions",
                columns: table => new
                {
                    SubscriptionId = table.Column<Guid>(nullable: false),
                    OfferId = table.Column<string>(nullable: true),
                    PlanId = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    SubscriptionName = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    LastOperationTime = table.Column<DateTimeOffset>(nullable: false),
                    SkuId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerSubscriptions", x => x.SubscriptionId);
                    table.ForeignKey(
                        name: "FK_CustomerSubscriptions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerSubscriptions_Skus_SkuId",
                        column: x => x.SkuId,
                        principalTable: "Skus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Company = table.Column<string>(nullable: true),
                    CustomerUserId = table.Column<Guid>(nullable: false),
                    Operation = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usages_CustomerUsers_CustomerUserId",
                        column: x => x.CustomerUserId,
                        principalTable: "CustomerUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Skus",
                columns: new[] { "Id", "Description", "MonthlyCost", "MonthlyLimit", "Name", "SalesChannel" },
                values: new object[,]
                {
                    { new Guid("885e900e-1a22-49ac-b526-e4154a8a5bbf"), "None", 0.0, 2147483647, "None", 2 },
                    { new Guid("b74519a4-368f-4d93-bd8f-143584da399e"), "Silver", 9.9900000000000002, 2, "Silver", 0 },
                    { new Guid("b0f164a7-a4ad-44b9-9062-7ea51a8a002a"), "Gold", 19.989999999999998, 5, "Gold", 0 },
                    { new Guid("758759d4-8f48-42cd-9309-c4b0005061b2"), "Platinum", 29.989999999999998, 7, "Platinum", 0 },
                    { new Guid("ccbb97b7-835d-4532-b4d2-03b7bfca8ce3"), "Silver", 9.9900000000000002, 2, "Silver", 1 },
                    { new Guid("84e2bd3c-5405-4568-acad-28a84ff98db1"), "Gold", 19.989999999999998, 5, "Gold", 1 },
                    { new Guid("21084065-506d-43df-8553-595513c2883b"), "Platinum", 29.989999999999998, 7, "Platinum", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSubscriptions_CustomerId",
                table: "CustomerSubscriptions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSubscriptions_SkuId",
                table: "CustomerSubscriptions",
                column: "SkuId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerUsers_CustomerId",
                table: "CustomerUsers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Usages_CustomerUserId",
                table: "Usages",
                column: "CustomerUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerSubscriptions");

            migrationBuilder.DropTable(
                name: "Usages");

            migrationBuilder.DropTable(
                name: "Skus");

            migrationBuilder.DropTable(
                name: "CustomerUsers");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
