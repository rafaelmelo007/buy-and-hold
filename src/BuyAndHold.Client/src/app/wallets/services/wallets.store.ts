import { Injectable } from '@angular/core';
import { MessagesService } from '../../common/services/messages.service';
import { WalletService } from './wallets.service';
import { BehaviorSubject, catchError, Observable, shareReplay } from 'rxjs';
import { Wallet } from '../domain/wallet';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export default class WalletsStore {
  constructor(
    private messagesService: MessagesService,
    private walletService: WalletService,
    private router: Router
  ) {
    this.loadMyWallet();
  }

  private myWalletSubject = new BehaviorSubject<Wallet[] | null>(null);

  myWallets$: Observable<Wallet[] | null> = this.myWalletSubject.asObservable();

  loadMyWallet(): void {
    this.walletService
      .getWallets()
      .pipe(
        catchError((err) => {
          this.messagesService.error(err.error.detail, err.error.title);
          throw err;
        }),
        shareReplay()
      )
      .subscribe((res) => {
        this.myWalletSubject.next(res);
      });
  }

  addWallet(name: string): void {
    this.walletService
      .saveWallet({ walletId: 0, userId: 0, name: name, isDefault: false })
      .pipe(
        catchError((err) => {
          this.messagesService.error(err.error.detail, err.error.title);
          throw err;
        }),
        shareReplay()
      )
      .subscribe((res) => {
        this.loadMyWallet();
        this.router.navigate(['/pages/wallets']);
      });
  }

  removeWallet(walletId: number): void {
    this.walletService
      .removeWallet(walletId)
      .pipe(
        catchError((err) => {
          this.messagesService.error(err.error.detail, err.error.title);
          throw err;
        }),
        shareReplay()
      )
      .subscribe(() => {
        this.loadMyWallet();
      });
  }

  saveWallet(wallet: Wallet) {
    this.walletService
      .saveWallet(wallet)
      .pipe(
        catchError((err) => {
          this.messagesService.error(err.error.detail, err.error.title);
          throw err;
        }),
        shareReplay()
      )
      .subscribe((res) => {
        this.loadMyWallet();
        this.router.navigate(['/pages/wallets']);
      });
  }
}
