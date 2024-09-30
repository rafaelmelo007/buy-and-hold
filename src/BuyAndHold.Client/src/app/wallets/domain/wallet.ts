import { WalletSymbol } from "./wallet-symbol";

export interface Wallet {
  walletId: number;
  name: string;
  userId: number;
  symbols?: WalletSymbol[] | null;
  isDefault: boolean;
  totalAmount?: number;
  expectedTotalAmount?: number;
}
