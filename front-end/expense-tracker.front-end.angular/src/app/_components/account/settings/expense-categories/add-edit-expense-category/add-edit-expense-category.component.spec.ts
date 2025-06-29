import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditExpenseCategoryComponent } from './add-edit-expense-category.component';

describe('AddEditExpenseCategoryComponent', () => {
  let component: AddEditExpenseCategoryComponent;
  let fixture: ComponentFixture<AddEditExpenseCategoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddEditExpenseCategoryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddEditExpenseCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
