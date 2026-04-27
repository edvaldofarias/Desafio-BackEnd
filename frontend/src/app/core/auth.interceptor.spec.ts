import { TestBed } from '@angular/core/testing';
import { HttpClient, HttpErrorResponse, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { authInterceptor } from './auth.interceptor';
import { AuthService } from './auth.service';

describe('authInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let auth: AuthService;
  let routerSpy: jasmine.SpyObj<Router>;

  beforeEach(() => {
    localStorage.clear();
    routerSpy = jasmine.createSpyObj<Router>('Router', ['navigateByUrl']);
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
        { provide: Router, useValue: routerSpy }
      ]
    });
    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
    auth = TestBed.inject(AuthService);
  });

  afterEach(() => httpMock.verify());

  it('does not add Authorization header when no token', () => {
    http.get('/x').subscribe();
    const req = httpMock.expectOne('/x');
    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush({});
  });

  it('adds Bearer token when authenticated', () => {
    spyOn(auth, 'token').and.returnValue('abc');
    http.get('/y').subscribe();
    const req = httpMock.expectOne('/y');
    expect(req.request.headers.get('Authorization')).toBe('Bearer abc');
    req.flush({});
  });

  it('logs out and redirects on 401 when authenticated', () => {
    spyOn(auth, 'token').and.returnValue('abc');
    const logoutSpy = spyOn(auth, 'logout');

    http.get('/z').subscribe({ next: () => fail('should error'), error: (e: HttpErrorResponse) => expect(e.status).toBe(401) });
    httpMock.expectOne('/z').flush(null, { status: 401, statusText: 'Unauthorized' });

    expect(logoutSpy).toHaveBeenCalled();
    expect(routerSpy.navigateByUrl).toHaveBeenCalledWith('/');
  });

  it('does not logout on 401 when no token (login attempt)', () => {
    http.get('/login').subscribe({ next: () => fail(), error: () => undefined });
    httpMock.expectOne('/login').flush(null, { status: 401, statusText: 'Unauthorized' });

    expect(routerSpy.navigateByUrl).not.toHaveBeenCalled();
  });
});
