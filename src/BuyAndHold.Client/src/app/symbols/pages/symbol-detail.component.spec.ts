import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SymbolDetailPage } from './symbol-detail.component';

describe('SymbolDetailComponent', () => {
  let component: SymbolDetailPage;
  let fixture: ComponentFixture<SymbolDetailPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SymbolDetailPage],
    }).compileComponents();

    fixture = TestBed.createComponent(SymbolDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
