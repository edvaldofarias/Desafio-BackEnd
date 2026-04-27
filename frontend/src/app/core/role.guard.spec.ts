import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { roleGuard } from './role.guard';
import { AuthService, Role } from './auth.service';

describe('roleGuard', () => {
  let routerSpy: jasmine.SpyObj<Router>;
  let auth: jasmine.SpyObj<AuthService>;

  function configure(role: Role | null): void {
    routerSpy = jasmine.createSpyObj<Router>('Router', ['navigateByUrl']);
    auth = jasmine.createSpyObj<AuthService>('AuthService', [], { role: () => role } as Partial<AuthService>);
    TestBed.configureTestingModule({
      providers: [
        { provide: Router, useValue: routerSpy },
        { provide: AuthService, useValue: auth }
      ]
    });
  }

  function run(required: Role): boolean {
    const route = {} as ActivatedRouteSnapshot;
    const state = {} as RouterStateSnapshot;
    return TestBed.runInInjectionContext(() => {
      const result = roleGuard(required)(route, state);
      return result as boolean;
    });
  }

  it('allows when role matches', () => {
    configure('admin');
    expect(run('admin')).toBeTrue();
    expect(routerSpy.navigateByUrl).not.toHaveBeenCalled();
  });

  it('redirects to admin login when admin required but role is null', () => {
    configure(null);
    expect(run('admin')).toBeFalse();
    expect(routerSpy.navigateByUrl).toHaveBeenCalledWith('/admin/login');
  });

  it('redirects to motoboy login when entregador required but role is admin', () => {
    configure('admin');
    expect(run('entregador')).toBeFalse();
    expect(routerSpy.navigateByUrl).toHaveBeenCalledWith('/motoboy/login');
  });
});
