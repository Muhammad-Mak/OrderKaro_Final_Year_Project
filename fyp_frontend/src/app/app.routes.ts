import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard/analytics',
        canActivate: [roleGuard(['Admin'])],
        loadComponent: () =>
          import('./features/dashboard/analytics/analytics.component').then(m => m.AnalyticsComponent)
      },
      {
        path: 'orders/active',
        loadComponent: () =>
          import('./features/orders/active-orders/active-orders.component').then(m => m.ActiveOrdersComponent)
      },
      {
        path: 'menu',
        loadComponent: () =>
          import('./features/menu/menu-management/menu-management.component').then(m => m.MenuManagementComponent)
      },
      {
        path: 'top-up',
        loadComponent: () =>
          import('./features/top-up/top-up-form/top-up-form.component').then(m => m.TopUpFormComponent)
      }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
