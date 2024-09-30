import { Component, ViewChild } from '@angular/core';
import {
  Column,
  GridTableComponent,
} from '../../../common/components/grid-table/grid-table.component';
import { LoadingComponent } from '../../../common/components/loading/loading.component';
import { MarketMood } from '../../domain/market-mood';
import { MarketMoodStore } from '../../services/market-mood.store';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-mood',
  standalone: true,
  imports: [GridTableComponent, LoadingComponent, CommonModule],
  templateUrl: './mood.component.html',
  styleUrl: './mood.component.css',
})
export class MoodPage {
  @ViewChild(GridTableComponent) grid?: GridTableComponent;
  marketMood?: MarketMood | null;
  query = '';

  columns: Column[] = [
    {
      field: 'name',
      header: 'Name',
      width: '100px',
      useHyperlink: true,
      action: (data: Symbol) => {},
    },
    {
      field: 'lastPrice',
      header: 'LastPrice',
      width: '200px',
      textAlign: 'center',
    },
    {
      field: 'lastPriceDate',
      header: 'Last Update',
      width: '200px',
      textAlign: 'center',
    },
    {
      field: 'actionRequired',
      header: 'Action',
      width: '100px',
      textAlign: 'center',
      badgeConditions: {
        'value == "Buy"': 'txbold tx-success',
        'value == "Sell"': 'txbold tx-danger',
      },
      useBadge: true,
    },
    {
      field: 'opportunityDescription',
      header: 'Opportunity',
      width: '400px',
    },
    {
      field: 'score',
      header: 'Score',
      width: '120px',
      textAlign: 'center',
      sortColumn: true,
      sortDirection: 'desc',
    },
  ];

  actions = [
    {
      label: 'Edit Opportunity',
      icon: 'fa fa-edit',
      action: (data: Symbol) => {},
    },
  ];

  count?: number;

  loading?: boolean;

  constructor(private marketMoodStore: MarketMoodStore) {
    this.marketMoodStore.marketMood$.subscribe((res) => {
      this.marketMood = res;
    });
  }
}
