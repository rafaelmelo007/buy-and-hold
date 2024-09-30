import { Component } from '@angular/core';
import { Wallet } from '../domain/wallet';
import WalletsStore from '../services/wallets.store';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { WalletSymbolsViewerComponent } from '../components/wallet-symbols-viewer.component';
import { TabsComponent } from '../../common/components/tabs/tabs.component';
import { MenuOption } from '../../common/domain/menu-option';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-wallets',
  standalone: true,
  imports: [
    DatePipe,
    CurrencyPipe,
    CommonModule,
    WalletSymbolsViewerComponent,
    TabsComponent,
    FormsModule,
  ],
  templateUrl: './wallets.component.html',
  styleUrl: './wallets.component.css',
})
export class WalletsPage {
  wallets?: Wallet[] | null;
  selectedWallet?: Wallet;
  selectedWalletId?: number;
  walletName?: string | null;
  selectedSection = 'Symbols';

  tabs: MenuOption[] = [
    {
      label: 'Symbols',
      icon: 'fa fa-rss',
      selected: true,
      action: (res: MenuOption) => {
        this.selectedSection = res.label!;
      },
    },
  ];

  constructor(private walletsStore: WalletsStore, private router: Router) {
    this.walletsStore.myWallets$.subscribe((res) => {
      this.wallets = res;
      if (this.walletName) {
        this.selectedWallet = res?.find((x) => x.name == this.walletName);
      } else {
        this.selectedWallet = res?.find((x) => x.isDefault);
        if (!this.selectedWallet && res && res.length > 0) {
          this.selectedWallet = res[0];
        }
      }
      this.selectedWalletId = this.selectedWallet?.walletId;
    });
  }

  changeWallet(): void {
    if (this.selectedWalletId == 0) {
      this.walletName = prompt('Enter the name of the new wallet');
      if (!this.walletName) return;

      this.walletsStore.addWallet(this.walletName);
      return;
    }
    this.selectedWallet = this.wallets?.find(
      (x) => x.walletId == this.selectedWalletId
    );
  }

  editWallet(): void {
    this.router.navigate([
      '/pages/wallets/save-wallet/' + this.selectedWalletId,
    ]);
  }

  removeWallet(): void {
    const confirmed = confirm(
      `You are able to delete the wallet ${this.selectedWallet?.name}. Do you confirm it?`
    );
    if (!confirmed) return;
    this.walletName = null;
    this.walletsStore.removeWallet(this.selectedWalletId!);
  }

  getDescription(wallet: Wallet): string {
    const amount = wallet.totalAmount!;
    const expectedAmount = wallet.expectedTotalAmount!;
    const percent = (amount / expectedAmount) * 100;
    const label = ` --------- [ Amount: ${this.formatCurrency(
      amount
    )} (${percent.toFixed(2)}%) / Expected: ${this.formatCurrency(
      expectedAmount
    )} ]`;
    return label;
  }

  formatCurrency(price: number): string {
    let formattedCurrency = new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'BRL',
    }).format(price);
    return formattedCurrency;
  }
}
