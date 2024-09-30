import { Component } from '@angular/core';
import { LoadingComponent } from '../../common/components/loading/loading.component';
import {
  Column,
  GridTableComponent,
} from '../../common/components/grid-table/grid-table.component';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-symbol-detail',
  standalone: true,
  imports: [LoadingComponent, GridTableComponent, CommonModule],
  templateUrl: './symbol-detail.component.html',
  styleUrl: './symbol-detail.component.css',
})
export class SymbolDetailPage {
  loading?: boolean;

  columns: Column[] = [
    {
      field: 'strategyName',
      header: 'StrategyName',
      width: '220px',
      useHyperlink: true, // This column will use hyperlink formatting
    },
  ];

  actions = [];

  symbol?: string | null;

  constructor(private route: ActivatedRoute) {
    this.route.paramMap.subscribe((params) => {
      this.symbol = params.get('symbol');
    });
  }
}
