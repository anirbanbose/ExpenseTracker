import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'et-page-title-bar',
  imports: [CommonModule, RouterLink],
  templateUrl: './page-title-bar.component.html',
  styleUrl: './page-title-bar.component.css'
})
export class PageTitleBarComponent {
  @Input() breadcrumbItems: { label: string, link: string | null }[] = [];
  @Input() pageTitle: string = '';
}
