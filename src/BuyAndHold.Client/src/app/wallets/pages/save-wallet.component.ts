import { AfterViewInit, Component, ViewChild } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Wallet } from '../domain/wallet';
import WalletsStore from '../services/wallets.store';
import { CommonModule } from '@angular/common';
import { WalletSymbolsEditorComponent } from '../components/wallet-symbols-editor.component';
import { TabsComponent } from '../../common/components/tabs/tabs.component';
import { MenuOption } from '../../common/domain/menu-option';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-save-wallet',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    WalletSymbolsEditorComponent,
    TabsComponent,
    FormsModule,
  ],
  templateUrl: './save-wallet.component.html',
  styleUrl: './save-wallet.component.css',
})
export class SaveWalletPage implements AfterViewInit {
  walletForm = new FormGroup({
    walletId: new FormControl(0),
    name: new FormControl('', [Validators.required]),
    userId: new FormControl(0),
  });

  loading: boolean = false;
  selectedSection?: string;
  wallets: Wallet[] | null = null;
  selectedWallet?: Wallet | null = null;
  selectedWalletId?: number | null = null;
  @ViewChild(WalletSymbolsEditorComponent)
  walletSymbolEditor?: WalletSymbolsEditorComponent;

  tabs: MenuOption[] = [
    {
      label: 'Symbols',
      icon: 'fa fa-rss',
      selected: true,
      action: (res: MenuOption) => {
        this.selectedSection = res.label;
      },
    },
  ];

  constructor(
    private walletsStore: WalletsStore,
    private route: ActivatedRoute
  ) {}

  ngAfterViewInit(): void {
    this.loading = true;

    const wallets$ = this.walletsStore.myWallets$;
    wallets$.subscribe((res) => {
      if (!res) return;
      if (!this.walletSymbolEditor) return;

      this.selectedWalletId = parseInt(this.route.snapshot.paramMap.get('id')!);

      this.wallets = res;
      this.selectedWallet = res?.find(
        (x) => x.walletId == this.selectedWalletId
      );
      if (!this.selectedWallet) return;

      this.walletForm.patchValue(this.selectedWallet);

      const symbols = this.selectedWallet?.symbols;
      this.walletSymbolEditor!.setWalletSymbols(symbols! ?? []);
      this.loading = false;
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.walletForm.valid) {
      const wallet = {
        ...this.walletForm.value,
        walletId: this.selectedWallet?.walletId,
        userId: this.selectedWallet?.userId,
        isDefault: this.selectedWallet?.isDefault,
        symbols: this.walletSymbolEditor?.walletSymbols,
      } as Wallet;
      this.walletsStore.saveWallet(JSON.parse(JSON.stringify(wallet)));
    }
  }
}
