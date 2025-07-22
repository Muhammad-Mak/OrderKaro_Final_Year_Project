// auth.guard.ts
// This route guard prevents access to protected routes unless the user is logged in.
// If the user is not authenticated, they are redirected to the login page.

import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

// Functional route guard that checks authentication status
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService); // Inject AuthService dynamically
  const router = inject(Router);           // Inject Router for redirection

  // If the user is not logged in, redirect to the login page
  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }

  // Allow navigation if user is authenticated
  return true;
};
