import { Component, Input } from '@angular/core';
import {
  Column,
  GridTableComponent,
} from '../../common/components/grid-table/grid-table.component';
import { WalletSymbol } from '../domain/wallet-symbol';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-wallet-symbols-viewer',
  standalone: true,
  imports: [GridTableComponent, CommonModule],
  templateUrl: './wallet-symbols-viewer.component.html',
  styleUrl: './wallet-symbols-viewer.component.css',
})
export class WalletSymbolsViewerComponent {
  loading?: boolean;
  @Input() dataSource?: WalletSymbol[];

  columns: Column[] = [
    {
      field: 'symbol',
      header: 'Symbol',
      width: '220px',
    },
    {
      field: 'quantity',
      header: 'Quantity',
      width: '300px',
      textAlign: 'center',
    },
    {
      field: 'averagePrice',
      header: 'Average Price',
      width: '300px',
      textAlign: 'center',
    },
    {
      field: 'totalAmount',
      header: 'Total Amount',
      width: '300px',
      textAlign: 'center',
    },
    {
      field: 'expectedQuantity',
      header: 'Expected Quantity',
      width: '300px',
      textAlign: 'center',
    },
    {
      field: 'expectedTotalAmount',
      header: 'Expected Total Amount',
      width: '300px',
      textAlign: 'center',
    },
    {
      field: 'percentCompleted',
      header: '% Completed',
      width: '300px',
      textAlign: 'center',
      sortColumn: true,
      sortDirection: 'desc',
    },
  ];
}
