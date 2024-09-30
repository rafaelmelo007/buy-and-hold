import { Injectable } from '@angular/core';
import { SymbolsService } from './symbols-service';
import { Symbol } from '../domain/symbol-model';
import { BehaviorSubject, catchError, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class SymbolsStore {
  constructor(private symbolsService: SymbolsService, private router: Router) {
    this.symbolsService
      .loadAllSymbols()

      .pipe(
        tap((symbols) => this.symbolsSubject.next(symbols)),
        catchError((err) => {
          this.router.navigate(['/pages/account/login']);
          throw err;
        })
      )
      .subscribe();
  }

  private symbolsSubject = new BehaviorSubject<Symbol[]>([]);
  symbols$: Observable<Symbol[]> = this.symbolsSubject.asObservable();
}
