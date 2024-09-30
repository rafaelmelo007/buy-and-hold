import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MarketBreadthComponent } from './market-breadth.component';

describe('MarketBreadthComponent', () => {
  let component: MarketBreadthComponent;
  let fixture: ComponentFixture<MarketBreadthComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MarketBreadthComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MarketBreadthComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
