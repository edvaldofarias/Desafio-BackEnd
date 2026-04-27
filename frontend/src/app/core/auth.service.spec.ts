import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';

describe('AuthService', () => {
  let service: AuthService;
  let http: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(AuthService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('starts unauthenticated', () => {
    expect(service.isAuthenticated()).toBeFalse();
    expect(service.role()).toBeNull();
    expect(service.token()).toBeNull();
  });

  it('persists admin session after login', () => {
    service.loginAdmin('a@a.com', 'pwd').subscribe();
    const req = http.expectOne(`${environment.apiUrl}/Manager/Authentication`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email: 'a@a.com', password: 'pwd' });
    req.flush({ token: 't1', data: { id: '1', email: 'a@a.com' } });

    expect(service.isAuthenticated()).toBeTrue();
    expect(service.role()).toBe('admin');
    expect(service.token()).toBe('t1');
    expect(service.identity()).toBe('a@a.com');
  });

  it('persists motoboy session and exposes id', () => {
    service.loginMotoboy('00000000000000', 'pwd').subscribe();
    const req = http.expectOne(`${environment.apiUrl}/entregadores/authentication`);
    req.flush({ token: 't2', data: { identifier: 'm-1', cnpj: '00000000000000', name: 'João' } });

    expect(service.role()).toBe('entregador');
    expect(service.motoboyId()).toBe('m-1');
    expect(service.displayName()).toBe('João');
    expect(service.token()).toBe('t2');
  });

  it('logout clears state and storage', () => {
    service.loginAdmin('x@x.com', 'p').subscribe();
    http.expectOne(() => true).flush({ token: 't', data: { id: '1', email: 'x@x.com' } });

    service.logout();

    expect(service.isAuthenticated()).toBeFalse();
    expect(localStorage.getItem('mottu.auth')).toBeNull();
  });
});
