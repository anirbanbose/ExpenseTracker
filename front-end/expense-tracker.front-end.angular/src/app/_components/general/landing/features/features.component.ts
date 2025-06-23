import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FeatureComponent } from '../../common/feature/feature.component';

@Component({
  selector: 'et-landing-features',
  imports: [CommonModule, FeatureComponent],
  templateUrl: './features.component.html',
  styleUrl: './features.component.css'
})
export class FeaturesComponent {
  features = [
    {
      title: 'Real-time Tracking',
      description: 'Monitor your daily expenses effortlessly.',
      icon: 'schedule',
    },
    {
      title: 'Budget Planning',
      description: 'Set budgets and get alerts before overspending.',
      icon: 'wallet',
    },
    {
      title: 'Insightful Reports',
      description: 'Visualize where your money goes each month.',
      icon: 'bar_chart',
    }
  ];
}
