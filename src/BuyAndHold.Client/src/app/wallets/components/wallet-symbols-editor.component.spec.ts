import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WalletSymbolsEditorComponent } from './wallet-symbols-editor.component';

describe('WalletSymbolsEditorComponent', () => {
  let component: WalletSymbolsEditorComponent;
  let fixture: ComponentFixture<WalletSymbolsEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WalletSymbolsEditorComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WalletSymbolsEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
