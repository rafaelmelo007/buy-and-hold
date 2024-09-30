import { Component, OnInit } from '@angular/core';
import {
  DropdownComponent,
} from '../../common/components/dropdown/dropdown.component';
import {
  Column,
  GridTableComponent,
} from '../../common/components/grid-table/grid-table.component';
import { SymbolsStore } from '../services/symbols-store';
import { Symbol } from '../domain/symbol-model';
import { LoadingComponent } from '../../common/components/loading/loading.component';
import { CommonModule } from '@angular/common';
import { MenuOption } from '../../common/domain/menu-option';

@Component({
  selector: 'app-symbols',
  standalone: true,
  imports: [
    DropdownComponent,
    GridTableComponent,
    LoadingComponent,
    CommonModule,
  ],
  templateUrl: './symbols.component.html',
  styleUrl: './symbols.component.css',
})
export class SymbolsPage implements OnInit {
  loading?: boolean;
  dataSource?: Symbol[];

  constructor(
    private symbolsStore: SymbolsStore,
  ) {}

  ngOnInit() {
    this.symbolsStore.symbols$.subscribe((res) => {
      this.loading = !res;
      this.dataSource = res;
    });
  }

  actions: MenuOption[] = [
    {
      label: 'Download Chart',
      icon: 'fa fa-download',
      action: (row) => {
      },
    },
  ];

  columns: Column[] = [
    {
      field: 'name',
      header: 'Symbol',
      width: '220px',
    },
    {
      field: 'lastPrice',
      header: 'Price',
      width: '160px',
      textAlign: 'center',
      badgeConditions: {
        'value > 0': 'txbold tx-success',
        'value < 0': 'txbold tx-danger',
      },
      useBadge: true, // This column will use badge formatting
    },
    {
      field: 'lastPriceDate',
      header: 'Last Update',
      width: '150px',
      textAlign: 'center',
    },
    {
      field: 'last30DaysVariation',
      header: '30D %',
      width: '150px',
      textAlign: 'center',
      badgeConditions: {
        'value > 0': 'txbold tx-success',
        'value < 0': 'txbold tx-danger',
      },
      useBadge: true, // This column will use badge formatting
    },
    {
      field: 'last100DaysVariation',
      header: '100D %',
      width: '150px',
      textAlign: 'center',
      badgeConditions: {
        'value > 0': 'txbold tx-success',
        'value < 0': 'txbold tx-danger',
      },
      useBadge: true, // This column will use badge formatting
      sortColumn: true,
    },
    {
      field: 'last6MonthsVariation',
      header: '6M %',
      width: '150px',
      textAlign: 'center',
      badgeConditions: {
        'value > 0': 'txbold tx-success',
        'value < 0': 'txbold tx-danger',
      },
      useBadge: true, // This column will use badge formatting
    },
  ];
}
