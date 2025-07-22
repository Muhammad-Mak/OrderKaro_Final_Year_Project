// jwt.interceptor.ts
// This HTTP interceptor automatically attaches the JWT token to all outgoing API requests,
// except for login and registration routes. It ensures authenticated requests to protected endpoints.

import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

// Define the interceptor as a functional interceptor using Angular 15+ syntax
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);           // Dynamically inject AuthService
  const token = authService.getToken();              // Get JWT token from local storage

  // Determine if the request is targeting your backend API
  const isApiUrl =
    req.url.startsWith('https://smartcafebackend.azurewebsites.net/api') ||
    req.url.startsWith('/api/');

  // Exclude login and register requests from interception
  const isAuthRoute = req.url.includes('/auth/login') || req.url.includes('/auth/register');

  // If the request is an API call and the user is authenticated (and it's not auth-related),
  // clone the request and add the Authorization header
  if (token && isApiUrl && !isAuthRoute) {
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(authReq); // Forward the modified request
  }

  // For all other cases, forward the original request unmodified
  return next(req);
};
