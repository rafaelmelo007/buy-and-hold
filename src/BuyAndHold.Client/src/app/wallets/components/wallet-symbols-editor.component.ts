import { Component, Input, OnInit } from '@angular/core';
import {
  FormArray,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { WalletSymbol } from '../domain/wallet-symbol';
import { SymbolsStore } from '../../symbols/services/symbols-store';
import { Symbol } from '../../symbols/domain/symbol-model';

@Component({
  selector: 'app-wallet-symbols-editor',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './wallet-symbols-editor.component.html',
  styleUrl: './wallet-symbols-editor.component.css',
})
export class WalletSymbolsEditorComponent implements OnInit {
  walletSymbolsForm = new FormGroup({
    walletSymbols: new FormArray([]),
  });

  @Input() walletId?: number;

  symbols?: Symbol[] | null;

  constructor(private symbolsStore: SymbolsStore) {}

  ngOnInit(): void {
    const symbols$ = this.symbolsStore.symbols$;

    symbols$.subscribe((res) => {
      this.symbols = res;
    });
  }

  get walletSymbolsArray(): FormArray {
    return this.walletSymbolsForm.get('walletSymbols') as FormArray;
  }

  get walletSymbols(): WalletSymbol[] {
    return this.walletSymbolsArray.controls.map((row: any) => {
      const rowControls = row.controls;
      return {
        symbol: rowControls.symbol.value,
        quantity: rowControls.quantity.value,
        averagePrice: rowControls.averagePrice.value,
        expectedQuantity: rowControls.expectedQuantity.value,
      } as WalletSymbol;
    });
  }

  setWalletSymbols(walletSymbols: WalletSymbol[] | null): void {
    this.walletSymbolsArray.clear();
    if (walletSymbols == null) return;

    walletSymbols.forEach((walletSymbol) => {
      const walletSymbolGroup = new FormGroup({
        symbol: new FormControl(walletSymbol.symbol, [Validators.required]),
        quantity: new FormControl(walletSymbol.quantity, [Validators.required]),
        averagePrice: new FormControl(walletSymbol.averagePrice, [
          Validators.required,
        ]),
        expectedQuantity: new FormControl(walletSymbol.expectedQuantity, [
          Validators.required,
        ]),
      });
      this.walletSymbolsArray.push(walletSymbolGroup);
    });
  }

  addWalletSymbol(): void {
    const walletSymbolGroup = new FormGroup({
      symbol: new FormControl('', [Validators.required]),
      quantity: new FormControl(100, [Validators.required]),
      averagePrice: new FormControl(0, [Validators.required]),
      expectedQuantity: new FormControl(100, [Validators.required]),
    });
    this.walletSymbolsArray.push(walletSymbolGroup);
  }

  removeWalletSymbol(index: number): void {
    this.walletSymbolsArray.removeAt(index);
  }
}
