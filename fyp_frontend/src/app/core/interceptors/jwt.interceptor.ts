import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  const isApiUrl =
    req.url.startsWith('https://smartcafebackend.azurewebsites.net/api') ||
    req.url.startsWith('/api/');
  const isAuthRoute = req.url.includes('/auth/login') || req.url.includes('/auth/register');

  if (token && isApiUrl && !isAuthRoute) {
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(authReq);
  }

  return next(req);
};
