import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { TestimonialComponent } from '../../common/testimonial/testimonial.component';

@Component({
  selector: 'et-landing-testimonials',
  imports: [
    CommonModule,
    TestimonialComponent
  ],
  templateUrl: './testimonials.component.html',
  styleUrl: './testimonials.component.css'
})
export class TestimonialsComponent {
  testimonials = [
    {
      name: 'Sarah M.',
      comment: 'This app changed the way I manage my money. Highly recommended!',
      avatarUrl: 'https://randomuser.me/api/portraits/women/68.jpg',
    },
    {
      name: 'James R.',
      comment: 'Clean, simple, and powerful. Perfect for daily use.',
      avatarUrl: 'https://randomuser.me/api/portraits/men/45.jpg',
    },
    {
      name: 'Emily K.',
      comment: "I've saved hundreds just by tracking my spending here.",
      avatarUrl: 'https://randomuser.me/api/portraits/women/44.jpg',
    },
  ];
}
