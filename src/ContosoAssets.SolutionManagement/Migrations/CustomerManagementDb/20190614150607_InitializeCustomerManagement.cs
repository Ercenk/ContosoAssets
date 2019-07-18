using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContosoAssets.SolutionManagement.Migrations.CustomerManagementDb
{
    public partial class InitializeCustomerManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Customers",
                table => new {Id = table.Column<Guid>(), CustomerName = table.Column<string>(nullable: true)},
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "Skus",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Description = table.Column<string>(nullable: true),
                    MonthlyCost = table.Column<double>(),
                    MonthlyLimit = table.Column<int>(),
                    Name = table.Column<string>(nullable: true),
                    SalesChannel = table.Column<int>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "CustomerUsers",
                table => new
                {
                    Id = table.Column<Guid>(),
                    CreatedDate = table.Column<DateTimeOffset>(),
                    CustomerId = table.Column<Guid>(),
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
                        "FK_CustomerUsers_Customers_CustomerId",
                        x => x.CustomerId,
                        "Customers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "CustomerSubscriptions",
                table => new
                {
                    SubscriptionId = table.Column<Guid>(),
                    OfferId = table.Column<string>(nullable: true),
                    PlanId = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(),
                    State = table.Column<int>(),
                    SubscriptionName = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(),
                    CustomerId = table.Column<Guid>(),
                    LastOperationTime = table.Column<DateTimeOffset>(),
                    SkuId = table.Column<Guid>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerSubscriptions", x => x.SubscriptionId);
                    table.ForeignKey(
                        "FK_CustomerSubscriptions_Customers_CustomerId",
                        x => x.CustomerId,
                        "Customers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_CustomerSubscriptions_Skus_SkuId",
                        x => x.SkuId,
                        "Skus",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Usages",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Company = table.Column<string>(nullable: true),
                    CustomerUserId = table.Column<Guid>(),
                    Operation = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTimeOffset>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usages", x => x.Id);
                    table.ForeignKey(
                        "FK_Usages_CustomerUsers_CustomerUserId",
                        x => x.CustomerUserId,
                        "CustomerUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                "Skus",
                new[] {"Id", "Description", "MonthlyCost", "MonthlyLimit", "Name", "SalesChannel"},
                new object[,]
                {
                    {new Guid("3d627c84-a37f-4bc7-be35-7c7e006481d1"), "None", 0.0, 2147483647, "None", 2},
                    {
                        new Guid("77229b05-7d60-4202-85a6-db5a1f94c1c8"), "Silver", 9.9900000000000002, 2, "Silver",
                        0
                    },
                    {new Guid("8b542189-d625-4982-8372-75ebaf1bb164"), "Gold", 19.989999999999998, 5, "Gold", 0},
                    {
                        new Guid("7e7144bf-607b-48e3-a380-0983abb8037f"), "Platinum", 29.989999999999998, 7,
                        "Platinum", 0
                    },
                    {
                        new Guid("a08be29f-4ff1-42ca-b195-f4f8bafb8ef4"), "Silver", 9.9900000000000002, 2, "Silver",
                        1
                    },
                    {new Guid("c22c5542-41a4-4346-8956-bb087a760ffb"), "Gold", 19.989999999999998, 5, "Gold", 1},
                    {
                        new Guid("c92186b9-0341-4868-8c5a-9c38a358cc56"), "Platinum", 29.989999999999998, 7,
                        "Platinum", 1
                    }
                });

            migrationBuilder.CreateIndex(
                "IX_CustomerSubscriptions_CustomerId",
                "CustomerSubscriptions",
                "CustomerId");

            migrationBuilder.CreateIndex(
                "IX_CustomerSubscriptions_SkuId",
                "CustomerSubscriptions",
                "SkuId");

            migrationBuilder.CreateIndex(
                "IX_CustomerUsers_CustomerId",
                "CustomerUsers",
                "CustomerId");

            migrationBuilder.CreateIndex(
                "IX_Usages_CustomerUserId",
                "Usages",
                "CustomerUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "CustomerSubscriptions");

            migrationBuilder.DropTable(
                "Usages");

            migrationBuilder.DropTable(
                "Skus");

            migrationBuilder.DropTable(
                "CustomerUsers");

            migrationBuilder.DropTable(
                "Customers");
        }
    }
}
