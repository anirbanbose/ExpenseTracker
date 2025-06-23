import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpenseExportDialogComponent } from './expense-export-dialog.component';

describe('ExpenseExportDialogComponent', () => {
  let component: ExpenseExportDialogComponent;
  let fixture: ComponentFixture<ExpenseExportDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ExpenseExportDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExpenseExportDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
