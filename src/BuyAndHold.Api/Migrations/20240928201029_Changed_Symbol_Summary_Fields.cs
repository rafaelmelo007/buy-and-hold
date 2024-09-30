using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuyAndHold.Api.Migrations
{
    /// <inheritdoc />
    public partial class Changed_Symbol_Summary_Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Last6MonthsPrice",
                table: "Symbols",
                newName: "PercentToLow");

            migrationBuilder.RenameColumn(
                name: "Last30DaysPrice",
                table: "Symbols",
                newName: "PercentToHigh");

            migrationBuilder.RenameColumn(
                name: "Last100DaysPrice",
                table: "Symbols",
                newName: "Candle300DLow");

            migrationBuilder.AddColumn<double>(
                name: "Candle100DHigh",
                table: "Symbols",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Candle100DLow",
                table: "Symbols",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Candle200DHigh",
                table: "Symbols",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Candle200DLow",
                table: "Symbols",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Candle300DHigh",
                table: "Symbols",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Candle100DHigh",
                table: "Symbols");

            migrationBuilder.DropColumn(
                name: "Candle100DLow",
                table: "Symbols");

            migrationBuilder.DropColumn(
                name: "Candle200DHigh",
                table: "Symbols");

            migrationBuilder.DropColumn(
                name: "Candle200DLow",
                table: "Symbols");

            migrationBuilder.DropColumn(
                name: "Candle300DHigh",
                table: "Symbols");

            migrationBuilder.RenameColumn(
                name: "PercentToLow",
                table: "Symbols",
                newName: "Last6MonthsPrice");

            migrationBuilder.RenameColumn(
                name: "PercentToHigh",
                table: "Symbols",
                newName: "Last30DaysPrice");

            migrationBuilder.RenameColumn(
                name: "Candle300DLow",
                table: "Symbols",
                newName: "Last100DaysPrice");
        }
    }
}
