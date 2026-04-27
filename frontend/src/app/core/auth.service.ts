import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { formatCnpj } from './mask.util';

export type Role = 'admin' | 'entregador';

interface AuthState {
  token: string;
  role: Role;
  identity: string;
  displayName: string;
  motoboyId?: string;
}

const STORAGE_KEY = 'mottu.auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly state = signal<AuthState | null>(this.read());

  readonly isAuthenticated = computed(() => this.state() !== null);
  readonly role = computed(() => this.state()?.role ?? null);
  readonly identity = computed(() => this.state()?.identity ?? null);
  readonly displayName = computed(() => this.state()?.displayName ?? null);
  readonly motoboyId = computed(() => this.state()?.motoboyId ?? null);

  loginAdmin(email: string, password: string): Observable<{ token: string; data: { id: string; email: string } }> {
    return this.http.post<{ token: string; data: { id: string; email: string } }>(
      `${environment.apiUrl}/Manager/Authentication`,
      { email, password }
    ).pipe(tap(res => this.persist({
      token: res.token,
      role: 'admin',
      identity: res.data.email,
      displayName: res.data.email
    })));
  }

  loginMotoboy(cnpj: string, password: string): Observable<{ token: string; data: { identifier: string; cnpj: string; name: string } }> {
    return this.http.post<{ token: string; data: { identifier: string; cnpj: string; name: string } }>(
      `${environment.apiUrl}/entregadores/authentication`,
      { cnpj, password }
    ).pipe(tap(res => this.persist({
      token: res.token,
      role: 'entregador',
      identity: formatCnpj(res.data.cnpj),
      displayName: res.data.name || formatCnpj(res.data.cnpj),
      motoboyId: res.data.identifier
    })));
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
    this.state.set(null);
  }

  token(): string | null {
    return this.state()?.token ?? null;
  }

  private persist(s: AuthState): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(s));
    this.state.set(s);
  }

  private read(): AuthState | null {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    try { return JSON.parse(raw) as AuthState; } catch { return null; }
  }
}
