import { Routes } from '@angular/router';
import { roleGuard } from './core/role.guard';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent) },

  { path: 'admin/login', loadComponent: () => import('./pages/admin/admin-login.component').then(m => m.AdminLoginComponent) },
  {
    path: 'admin/motos',
    canActivate: [roleGuard('admin')],
    loadComponent: () => import('./pages/admin/admin-motos.component').then(m => m.AdminMotosComponent)
  },

  { path: 'motoboy/login', loadComponent: () => import('./pages/motoboy/motoboy-login.component').then(m => m.MotoboyLoginComponent) },
  { path: 'motoboy/cadastro', loadComponent: () => import('./pages/motoboy/motoboy-register.component').then(m => m.MotoboyRegisterComponent) },
  {
    path: 'motoboy/painel',
    canActivate: [roleGuard('entregador')],
    loadComponent: () => import('./pages/motoboy/motoboy-dashboard.component').then(m => m.MotoboyDashboardComponent)
  },

  { path: '**', redirectTo: '' }
];
