import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { Symbol } from '../domain/symbol-model';
import { appSettings } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class SymbolsService {
  constructor(private http: HttpClient) {}

  loadAllSymbols(): Observable<Symbol[]> {
    let result = this.http
      .get<any>(`${appSettings.baseUrl}/symbols/get-symbols`)
      .pipe(
        map((res) => {
          return res['data'];
        })
      );
    return result;
  }
}
