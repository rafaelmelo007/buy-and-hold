import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SaveWalletComponent } from './save-wallet.component';

describe('SaveWalletComponent', () => {
  let component: SaveWalletComponent;
  let fixture: ComponentFixture<SaveWalletComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SaveWalletComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SaveWalletComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
