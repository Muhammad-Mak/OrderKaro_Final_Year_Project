import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export function roleGuard(allowedRoles: string[]): CanActivateFn {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    const userRole = authService.getUserRole();
    if (!allowedRoles.includes(userRole)) {
      router.navigate(['/login']);
      return false;
    }

    return true;
  };
}
// This guard checks if the user's role is included in the allowed roles.
// If not, it redirects to the login page.