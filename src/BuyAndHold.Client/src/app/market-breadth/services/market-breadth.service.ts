import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, of } from 'rxjs';
import { appSettings } from '../../environments/environment';
import { AverageLines } from '../domain/average-lines';

@Injectable({
  providedIn: 'root',
})
export class MarketBreadthService {
  constructor(private http: HttpClient) {}

  loadWalletBreadth(walletId: number): Observable<AverageLines[] | null> {
    const result = this.http
      .get<{ data: AverageLines[] | null }>(
        `${appSettings.baseUrl}/market-breadth/get-wallet-breadth?WalletId=${walletId}`
      )
      .pipe(map((res) => res.data));
    return result;
  }
}
