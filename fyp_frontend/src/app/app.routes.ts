// routes.ts
// This file defines the main routing configuration for the Angular application. 
// It sets up paths for authentication, protected areas with layout and role-based access, 
// and lazy loads components for various feature modules.

import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  // Redirect root path to 'login'
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  // Public routes for login and registration
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  // Protected routes using the MainLayoutComponent
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard], // Protects all child routes under this layout
    children: [
      {
        path: 'dashboard/analytics',
        canActivate: [roleGuard(['Admin'])], // Only Admin users can access this route
        loadComponent: () =>
          import('./features/dashboard/analytics/analytics.component')
            .then(m => m.AnalyticsComponent) // Lazy loading AnalyticsComponent
      },
      {
        path: 'orders/active',
        loadComponent: () =>
          import('./features/orders/active-orders/active-orders.component')
            .then(m => m.ActiveOrdersComponent) // Lazy loading ActiveOrdersComponent
      },
      {
        path: 'menu',
        loadComponent: () =>
          import('./features/menu/menu-management/menu-management.component')
            .then(m => m.MenuManagementComponent) // Lazy loading MenuManagementComponent
      },
      {
        path: 'top-up',
        loadComponent: () =>
          import('./features/top-up/top-up-form/top-up-form.component')
            .then(m => m.TopUpFormComponent) // Lazy loading TopUpFormComponent
      }
    ]
  },

  // Wildcard route to catch all undefined paths and redirect to login
  { path: '**', redirectTo: 'login' }
];
