import { Component, Input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'et-feature',
  imports: [
    MatCardModule,
    MatIconModule
  ],
  templateUrl: './feature.component.html',
  styleUrl: './feature.component.css'
})
export class FeatureComponent {
  @Input() feature: any = {};
}
