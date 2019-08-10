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
                    { new Guid("097af8ee-f0a3-4d94-9a2b-56286f40ae26"), "None", 0.0, 2147483647, "None", 2 },
                    { new Guid("74b32656-6cea-4643-a62c-df18e18df51f"), "Silver", 9.9900000000000002, 2, "Silver", 0 },
                    { new Guid("1f1e7f72-c872-48c1-8fe1-10c2002c24a9"), "Gold", 19.989999999999998, 5, "Gold", 0 },
                    { new Guid("2b014a1b-367b-4c35-b3b9-49b9f64af818"), "Platinum", 29.989999999999998, 7, "Platinum", 0 },
                    { new Guid("f67febe9-547d-4c51-b82e-53bac023b938"), "Silver", 9.9900000000000002, 2, "Silver", 1 },
                    { new Guid("f9b76781-a083-45c3-91c5-95074b7c944a"), "Gold", 19.989999999999998, 5, "Gold", 1 },
                    { new Guid("0f9f58e5-43ed-4400-a967-17dfccceef8c"), "Platinum", 29.989999999999998, 7, "Platinum", 1 }
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
