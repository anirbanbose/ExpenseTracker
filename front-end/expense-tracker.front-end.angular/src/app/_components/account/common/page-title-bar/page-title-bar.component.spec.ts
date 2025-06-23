import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PageTitleBarComponent } from './page-title-bar.component';

describe('PageTitleBarComponent', () => {
  let component: PageTitleBarComponent;
  let fixture: ComponentFixture<PageTitleBarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PageTitleBarComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PageTitleBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
