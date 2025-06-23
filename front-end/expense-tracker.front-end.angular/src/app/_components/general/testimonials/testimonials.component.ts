import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { TestimonialComponent } from '../common/testimonial/testimonial.component';

@Component({
  selector: 'app-testimonials',
  imports: [
    CommonModule,
    MatCardModule,
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
    {
      name: 'Michael B.',
      comment: "A must-have tool for anyone serious about managing expenses.",
      avatarUrl: 'https://randomuser.me/api/portraits/men/33.jpg',
    },
    {
      name: 'Olivia P.',
      comment: "Simple interface and very effective. Love the insights!",
      avatarUrl: 'https://randomuser.me/api/portraits/women/52.jpg',
    },
    {
      name: 'David L.',
      comment: "The best app I’ve used to keep my finances organized and clear.",
      avatarUrl: 'https://randomuser.me/api/portraits/men/12.jpg',
    },
  ];
}
