using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Order.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CustomerEmail = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrderDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OrderStatus = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SingleItemOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ParentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SingleItemOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SingleItemOrders_Orders_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CustomerEmail", "OrderDate", "OrderStatus" },
                values: new object[] { new Guid("25430aac-71eb-4686-8a51-bcc396fcb4f6"), "rairacer@gmail", new DateTime(2025, 9, 4, 20, 20, 35, 736, DateTimeKind.Local).AddTicks(8902), "Pending Payment" });

            migrationBuilder.InsertData(
                table: "SingleItemOrders",
                columns: new[] { "Id", "ParentId", "ProductId", "Quantity" },
                values: new object[,]
                {
                    { new Guid("10fb6988-3b05-41bf-b66e-45fba6e38df0"), new Guid("25430aac-71eb-4686-8a51-bcc396fcb4f6"), new Guid("00a97beb-e222-4b1e-a303-64d415fcb1eb"), 10 },
                    { new Guid("9d48eb2a-10b7-437f-a4cb-2ede05fdddb6"), new Guid("25430aac-71eb-4686-8a51-bcc396fcb4f6"), new Guid("4ac5caf5-8151-48c7-a07a-9ec3a19cf510"), 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SingleItemOrders_ParentId",
                table: "SingleItemOrders",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SingleItemOrders");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
