using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuyAndHold.Api.Migrations
{
    /// <inheritdoc />
    public partial class WalletRefs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "WalletId",
                table: "WalletSymbols",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Wallets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_WalletSymbols_WalletId",
                table: "WalletSymbols",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletSymbols_Wallets_WalletId",
                table: "WalletSymbols",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletSymbols_Wallets_WalletId",
                table: "WalletSymbols");

            migrationBuilder.DropIndex(
                name: "IX_WalletSymbols_WalletId",
                table: "WalletSymbols");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Wallets");

            migrationBuilder.AlterColumn<long>(
                name: "WalletId",
                table: "WalletSymbols",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
