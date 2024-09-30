import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { appSettings } from '../../environments/environment';
import { Wallet } from '../domain/wallet';

@Injectable({
  providedIn: 'root',
})
export class WalletService {
  constructor(private http: HttpClient) {}

  getWallets(): Observable<Wallet[]> {
    let result = this.http
      .get<any>(`${appSettings.baseUrl}/wallets/get-wallets`)
      .pipe(
        map((res) => {
          return res['wallets'];
        })
      );
    return result;
  }

  saveWallet(wallet: Wallet): Observable<number> {
    let result = this.http.post<number>(
      `${appSettings.baseUrl}/wallets/save-wallet`,
      { wallet }
    );
    return result;
  }

  removeWallet(walletId: number): Observable<number> {
    let result = this.http.delete<number>(
      `${appSettings.baseUrl}/wallets/remove-wallet?WalletId=${walletId}`
    );
    return result;
  }
}
