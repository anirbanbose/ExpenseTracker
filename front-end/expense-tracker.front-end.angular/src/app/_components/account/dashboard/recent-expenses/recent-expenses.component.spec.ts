import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecentExpensesComponent } from './recent-expenses.component';

describe('RecentExpensesComponent', () => {
  let component: RecentExpensesComponent;
  let fixture: ComponentFixture<RecentExpensesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecentExpensesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RecentExpensesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
