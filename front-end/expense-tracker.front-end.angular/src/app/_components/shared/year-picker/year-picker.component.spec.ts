import { ComponentFixture, TestBed } from '@angular/core/testing';

import { YearPickerComponent } from './year-picker.component';

describe('YearDateRangePickerComponent', () => {
  let component: YearPickerComponent;
  let fixture: ComponentFixture<YearPickerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [YearPickerComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(YearPickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
