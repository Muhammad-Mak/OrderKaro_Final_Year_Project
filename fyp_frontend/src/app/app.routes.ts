import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { authGuard } from './core/guards/auth.guard';

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
        loadComponent: () =>
          import('./features/dashboard/analytics/analytics.component').then(m => m.AnalyticsComponent),
      },
      {
        path: 'dashboard/users',
        loadComponent: () =>
          import('./features/dashboard/users/users.component').then(m => m.UsersComponent),
      },
      {
        path: 'dashboard/orders',
        loadComponent: () =>
          import('./features/dashboard/orders/orders.component').then(m => m.OrdersComponent),
      },
      {
        path: 'orders/active',
        loadComponent: () =>
          import('./features/orders/active-orders/active-orders.component').then(m => m.ActiveOrdersComponent),
      },
      {
        path: 'menu/items',
        loadComponent: () =>
          import('./features/menu/items/items.component').then(m => m.ItemsComponent),
      },
      {
        path: 'menu/categories',
        loadComponent: () =>
          import('./features/menu/categories/categories.component').then(m => m.CategoriesComponent),
      },
      {
        path: 'top-up',
        loadComponent: () =>
          import('./features/top-up/top-up-form/top-up-form.component').then(m => m.TopUpFormComponent),
      },
      { path: '', redirectTo: 'dashboard/analytics', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
