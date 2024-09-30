using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuyAndHold.Api.Migrations
{
    /// <inheritdoc />
    public partial class SymbolAverages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Average200",
                table: "DailyStockPrices",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Average21",
                table: "DailyStockPrices",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Average50",
                table: "DailyStockPrices",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Average200",
                table: "DailyStockPrices");

            migrationBuilder.DropColumn(
                name: "Average21",
                table: "DailyStockPrices");

            migrationBuilder.DropColumn(
                name: "Average50",
                table: "DailyStockPrices");
        }
    }
}
