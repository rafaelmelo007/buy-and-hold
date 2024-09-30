import { Component } from '@angular/core';
import WalletsStore from '../../../wallets/services/wallets.store';
import { Wallet } from '../../../wallets/domain/wallet';
import { AverageLines } from '../../domain/average-lines';
import { TabsComponent } from '../../../common/components/tabs/tabs.component';
import {
  Column,
  GridTableComponent,
} from '../../../common/components/grid-table/grid-table.component';
import { MarketBreadthStore } from '../../services/market-breadth.store';
import { CommonModule } from '@angular/common';
import { LoadingComponent } from '../../../common/components/loading/loading.component';
import { MenuOption } from '../../../common/domain/menu-option';

@Component({
  selector: 'app-market-breadth',
  standalone: true,
  imports: [TabsComponent, LoadingComponent, GridTableComponent, CommonModule],
  templateUrl: './market-breadth.component.html',
  styleUrl: './market-breadth.component.css',
})
export class MarketBreadthPage {
  wallets?: Wallet[] | null;
  selectedWallet?: Wallet;
  lines?: AverageLines[];

  columns: Column[] = [
    {
      field: 'date',
      header: 'Date',
      width: '100px',
    },
    {
      field: 'average21',
      header: 'Average21',
      width: '200px',
      textAlign: 'center',
    },
    {
      field: 'average50',
      header: 'Average50',
      width: '200px',
      textAlign: 'center',
    },
    {
      field: 'average200',
      header: 'Average200',
      width: '200px',
      textAlign: 'center',
    },
  ];

  tabs: MenuOption[] = [];

  loading = false;

  actions = [];

  constructor(
    walletsStore: WalletsStore,
    private marketBreadthStore: MarketBreadthStore
  ) {
    const wallets$ = walletsStore.myWallets$;
    wallets$.subscribe((res) => {
      this.wallets = res;
      if (!this.wallets) return;

      this.selectedWallet = this.wallets?.find((x) => x.isDefault);
      if (!this.selectedWallet && this.wallets && this.wallets?.length > 0) {
        this.selectedWallet = this.wallets[0];
      }

      this.tabs = this.wallets!.map((x) => {
        let tab = new MenuOption();
        tab.label = x.name;
        tab.icon = 'fa fa-briefcase';
        tab.selected = x.walletId == this.selectedWallet?.walletId;
        tab.action = (res) => {
          this.loadWalletBreadth(x.walletId);
        };
        return tab;
      });

      if (!this.selectedWallet) return;

      this.loadWalletBreadth(this.selectedWallet?.walletId);
    });
  }

  loadWalletBreadth(walletId: number): void {
    this.loading = true;
    this.marketBreadthStore.loadWalletBreadth(walletId).subscribe((res) => {
      this.lines = res ?? [];
      this.loading = false;
    });
  }
}
