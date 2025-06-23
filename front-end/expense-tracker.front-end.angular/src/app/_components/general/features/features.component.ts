import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FeatureComponent } from '../common/feature/feature.component';

@Component({
  selector: 'app-features',
  imports: [CommonModule, FeatureComponent],
  templateUrl: './features.component.html',
  styleUrl: './features.component.css'
})
export class FeaturesComponent {
  features = [
    {
      title: 'Real-time Tracking',
      icon: 'schedule',
      description: 'Keep tabs on your expenses as they happen, right from your dashboard.',
    },
    {
      title: 'Budget Planning',
      icon: 'wallet',
      description: 'Set monthly or custom budgets and get alerts when you’re nearing your limits.',
    },
    {
      title: 'Insightful Reports',
      icon: 'bar_chart',
      description: 'Access visual reports to understand your spending patterns better.',
    }
  ];
}
