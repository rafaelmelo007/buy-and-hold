import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WalletSymbolsViewerComponent } from './wallet-symbols-viewer.component';

describe('WalletSymbolsViewerComponent', () => {
  let component: WalletSymbolsViewerComponent;
  let fixture: ComponentFixture<WalletSymbolsViewerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WalletSymbolsViewerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WalletSymbolsViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
