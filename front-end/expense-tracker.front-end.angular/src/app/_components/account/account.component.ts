import { RouterModule, RouterOutlet } from '@angular/router';
import { FooterComponent } from '../shared/footer/footer.component';
import { HeaderComponent } from './common/header/header.component';
import { SidebarComponent } from './common/sidebar/sidebar.component';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
@Component({
  selector: 'app-account',
  imports: [
    RouterModule,
    RouterOutlet,
    CommonModule,
    MatSidenavModule,
    MatButtonModule,
    HeaderComponent,
    SidebarComponent,
    FooterComponent
  ],
  templateUrl: './account.component.html',
  styleUrl: './account.component.css',
})
export class AccountComponent implements OnInit {
  isLargeScreen = true;
  sidenavMode: 'side' | 'over' = 'side';

  constructor(private breakpointObserver: BreakpointObserver) { }

  ngOnInit(): void {
    this.breakpointObserver.observe([Breakpoints.Small, Breakpoints.XSmall])
      .subscribe(result => {
        this.isLargeScreen = !result.matches;
        this.sidenavMode = this.isLargeScreen ? 'side' : 'over';
      });
  }
}
