import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpenseAdvancedSearchComponent } from './expense-advanced-search.component';

describe('ExpenseAdvancedSearchComponent', () => {
  let component: ExpenseAdvancedSearchComponent;
  let fixture: ComponentFixture<ExpenseAdvancedSearchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ExpenseAdvancedSearchComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExpenseAdvancedSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
