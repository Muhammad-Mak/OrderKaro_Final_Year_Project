// role.guard.ts
// This route guard restricts access based on user roles (e.g., Admin, Staff).
// If the user's role is not in the list of allowed roles, they are redirected to the login page.

import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

// Factory function that takes allowed roles and returns a CanActivate guard
export function roleGuard(allowedRoles: string[]): CanActivateFn {
  return () => {
    const authService = inject(AuthService);  // Inject AuthService at runtime
    const router = inject(Router);            // Inject Router for navigation

    const userRole = authService.getUserRole(); // Get current user's role from localStorage

    // If user role is not allowed, redirect to login
    if (!allowedRoles.includes(userRole)) {
      router.navigate(['/login']);
      return false;
    }

    // Allow route access if role matches
    return true;
  };
}
