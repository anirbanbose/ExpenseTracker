import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'et-testimonial',
  imports: [MatCardModule],
  templateUrl: './testimonial.component.html',
  styleUrl: './testimonial.component.css'
})
export class TestimonialComponent {
  @Input() testimonial: any = {}
}
