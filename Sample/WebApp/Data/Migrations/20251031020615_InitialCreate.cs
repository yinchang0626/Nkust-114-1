using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentPerUnitWeight = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    IntegratedNumber = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    AnalysisCategory = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    SampleName = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    ContentPer100g = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    ContentPerUnit = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    StandardDeviation = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    UnitWeight = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    ContentUnit = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    SampleCount = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    WasteRate = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    SampleEnglishName = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    DataCategory = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    AnalysisItem = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    FoodCategory = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    ContentDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommonName = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodInfos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodInfos_AnalysisCategory",
                table: "FoodInfos",
                column: "AnalysisCategory");

            migrationBuilder.CreateIndex(
                name: "IX_FoodInfos_IntegratedNumber",
                table: "FoodInfos",
                column: "IntegratedNumber");

            migrationBuilder.CreateIndex(
                name: "IX_FoodInfos_SampleName",
                table: "FoodInfos",
                column: "SampleName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodInfos");
        }
    }
}
