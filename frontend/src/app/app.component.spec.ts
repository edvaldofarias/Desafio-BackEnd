import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { AppComponent } from './app.component';
import { AuthService } from './core/auth.service';

describe('AppComponent', () => {
  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()]
    });
  });

  it('renders the brand link', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const text: string = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('Mottu Test App');
  });

  it('shows logout button when authenticated', () => {
    const auth = TestBed.inject(AuthService);
    spyOn(auth, 'isAuthenticated').and.returnValue(true);
    spyOn(auth, 'role').and.returnValue('admin');
    spyOn(auth, 'identity').and.returnValue('a@a.com');

    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const btn = (fixture.nativeElement as HTMLElement).querySelector('button');
    expect(btn).toBeTruthy();
    expect(btn?.textContent).toContain('Sair');
  });
});
