import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { MarketMoodService } from './market-mood.service';
import { MarketMood } from '../domain/market-mood';

@Injectable({
  providedIn: 'root',
})
export class MarketMoodStore {
  constructor(private marketMoodService: MarketMoodService) {
    this.marketMoodService.loadMarketMood().subscribe((res) => {
      this.marketMoodSubject.next(res);
    });
  }

  private marketMoodSubject = new BehaviorSubject<MarketMood | null>(null);

  marketMood$: Observable<MarketMood | null> =
    this.marketMoodSubject.asObservable();
}
