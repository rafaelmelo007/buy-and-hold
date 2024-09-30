export interface WalletSymbol {
  walletSymbolId?: number;
  walletId: number;
  symbol: string;
  quantity: Date;
  averagePrice: string;
  expectedQuantity: number;
  totalAmount?: number;
  expectedTotalAmount?: number;
}
