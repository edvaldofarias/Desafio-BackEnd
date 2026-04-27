import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService, Role } from './auth.service';

export const roleGuard = (required: Role): CanActivateFn => () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (auth.role() === required) return true;
  router.navigateByUrl(required === 'admin' ? '/admin/login' : '/motoboy/login');
  return false;
};
