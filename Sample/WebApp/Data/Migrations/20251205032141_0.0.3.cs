using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleApp.Migrations
{
    /// <inheritdoc />
    public partial class _003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommonName",
                table: "Foods",
                type: "nvarchar(500)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentDescription",
                table: "Foods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Foods",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FoodCategory",
                table: "Foods",
                type: "nvarchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SampleEnglishName",
                table: "Foods",
                type: "nvarchar(500)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DataCategory",
                table: "AnalysisItems",
                type: "nvarchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AnalysisItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DefaultUnit",
                table: "AnalysisItems",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AnalysisItems",
                type: "nvarchar(500)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AnalysisItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FoodAnalysisItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodId = table.Column<int>(type: "int", nullable: false),
                    AnalysisItemId = table.Column<int>(type: "int", nullable: false),
                    ContentPer100g = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ContentPerUnit = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    ContentPerUnitWeight = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    StandardDeviation = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    SampleCount = table.Column<int>(type: "int", nullable: true),
                    ContentUnit = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    DataCategory = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UnitWeight = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    WasteRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodAnalysisItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodAnalysisItems_AnalysisItems_AnalysisItemId",
                        column: x => x.AnalysisItemId,
                        principalTable: "AnalysisItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FoodAnalysisItems_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Foods_IntegratedNumber",
                table: "Foods",
                column: "IntegratedNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Foods_SampleName",
                table: "Foods",
                column: "SampleName");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisItems_DataCategory_Name",
                table: "AnalysisItems",
                columns: new[] { "DataCategory", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodAnalysisItems_AnalysisItemId",
                table: "FoodAnalysisItems",
                column: "AnalysisItemId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodAnalysisItems_CreatedAt",
                table: "FoodAnalysisItems",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FoodAnalysisItems_FoodId_AnalysisItemId",
                table: "FoodAnalysisItems",
                columns: new[] { "FoodId", "AnalysisItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodAnalysisItems_IsActive",
                table: "FoodAnalysisItems",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodAnalysisItems");

            migrationBuilder.DropIndex(
                name: "IX_Foods_IntegratedNumber",
                table: "Foods");

            migrationBuilder.DropIndex(
                name: "IX_Foods_SampleName",
                table: "Foods");

            migrationBuilder.DropIndex(
                name: "IX_AnalysisItems_DataCategory_Name",
                table: "AnalysisItems");

            migrationBuilder.DropColumn(
                name: "CommonName",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "ContentDescription",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "FoodCategory",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "SampleEnglishName",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AnalysisItems");

            migrationBuilder.DropColumn(
                name: "DefaultUnit",
                table: "AnalysisItems");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AnalysisItems");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AnalysisItems");

            migrationBuilder.AlterColumn<string>(
                name: "DataCategory",
                table: "AnalysisItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)");
        }
    }
}
