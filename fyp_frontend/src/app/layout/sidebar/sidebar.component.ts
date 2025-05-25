import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent {
  userRole: string = '';

  constructor(public auth: AuthService, private router: Router) {
    this.userRole = this.auth.getUserRole();
  }

  navigateTo(path: string) {
    this.router.navigate([path]);
  }
  
  isActive(path: string): boolean {
    return this.router.url === path;
  }

  logout() {
    this.auth.logout();
  }
}
