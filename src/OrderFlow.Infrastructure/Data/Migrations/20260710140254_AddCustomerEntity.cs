using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "User",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserExternalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_UserExternalId",
                table: "Customer",
                column: "UserExternalId",
                unique: true);

            // ── VIEW 
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW vw_OrderWithCustomer AS
                SELECT
                    o.""Id"" AS OrderId,
                    o.""CustomerId"",
                    o.""Status"",
                    o.""Total_Amount"" AS TotalAmount,
                    o.""OrderDate"" AS OrderCreatedAt,
                    c.""Name"" AS CustomerName,
                    c.""Email"" AS CustomerEmail,
                    c.""Phone"" AS CustomerPhone
                FROM ""Orders"" o
                INNER JOIN ""Customer"" c ON c.""Id"" = o.""CustomerId"";
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ── REMOVE VIEW 
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_OrderWithCustomer;");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "User");
        }
    }
}