import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, of } from 'rxjs';
import { appSettings } from '../../environments/environment';
import { MarketMood } from '../domain/market-mood';

@Injectable({
  providedIn: 'root',
})
export class MarketMoodService {
  constructor(private http: HttpClient) {}

  loadMarketMood(): Observable<MarketMood> {
    const result = this.http
      .get<MarketMood>(`${appSettings.baseUrl}/market-mood/get-market-mood`)
      .pipe(map((res) => res));
    return result;
  }
}
