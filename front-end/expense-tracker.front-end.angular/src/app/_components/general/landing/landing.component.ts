import { Component } from '@angular/core';
import { HeroComponent } from './hero/hero.component';
import { FeaturesComponent } from './features/features.component';
import { TestimonialsComponent } from './testimonials/testimonials.component';
import { CallToActionComponent } from './call-to-action/call-to-action.component';

@Component({
  selector: 'app-landing',
  imports: [
    HeroComponent,
    FeaturesComponent,
    TestimonialsComponent,
    CallToActionComponent
  ],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.css'
})
export class LandingComponent {

}
