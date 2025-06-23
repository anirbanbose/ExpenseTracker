import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { fadeDropdown } from '../../_helpers/animations';
import { FooterComponent } from '../shared/footer/footer.component';
import { HeaderComponent } from './header/header.component';

@Component({
  selector: 'app-general',
  imports: [
    RouterOutlet,
    HeaderComponent,
    FooterComponent
  ],
  templateUrl: './general.component.html',
  styleUrl: './general.component.css',
  animations: [fadeDropdown],
})
export class GeneralComponent {

  constructor() {
  }
} 
