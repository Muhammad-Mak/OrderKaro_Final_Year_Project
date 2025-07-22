// sidebar.component.ts
// This component is responsible for rendering the application's sidebar navigation.
// It shows/hides elements based on the user's role, handles route navigation, 
// and provides logout functionality.

import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-sidebar',            // Component can be used via <app-sidebar></app-sidebar>
  standalone: true,                   // This is a standalone component (no module needed)
  imports: [CommonModule],           // Common directives like *ngIf, *ngFor are available
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent {
  userRole: string = '';             // Stores the current user's role (e.g., 'Admin', 'Customer')

  constructor(public auth: AuthService, private router: Router) {
    // On initialization, retrieve the user's role from the AuthService
    this.userRole = this.auth.getUserRole();
  }

  // Navigate to a specific route path when a menu item is clicked
  navigateTo(path: string) {
    this.router.navigate([path]);
  }

  // Checks whether a given route is the currently active route
  isActive(path: string): boolean {
    return this.router.url === path;
  }

  // Logs the user out using the AuthService
  logout() {
    this.auth.logout();
  }
}
