import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MarketBreadthService } from './market-breadth.service';
import { AverageLines } from '../domain/average-lines';

@Injectable({
  providedIn: 'root',
})
export class MarketBreadthStore {
  constructor(private marketBreadthService: MarketBreadthService) {}

  loadWalletBreadth(walletId: number): Observable<AverageLines[] | null> {
    return this.marketBreadthService.loadWalletBreadth(walletId);
  }
}
