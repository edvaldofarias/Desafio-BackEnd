import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/auth.service';
import { extractErrorMessage } from '../../core/error.util';

@Component({
  selector: 'app-motoboy-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    <section class="card" style="max-width:420px;margin:0 auto;">
      <h1>Login Entregador</h1>
      <form (submit)="submit($event)">
        <label for="cnpj">CNPJ</label>
        <input id="cnpj" name="cnpj" [(ngModel)]="cnpj" required />

        <label for="password">Senha</label>
        <input id="password" name="password" type="password" [(ngModel)]="password" required />

        <div style="margin-top:1.2rem;display:flex;gap:.6rem;">
          <button type="submit" class="btn" [disabled]="loading()">
            {{ loading() ? 'Entrando…' : 'Entrar' }}
          </button>
          <a class="btn btn-link" routerLink="/motoboy/cadastro">Cadastrar-se</a>
          <a class="btn btn-link" routerLink="/">Voltar</a>
        </div>
      </form>
      @if (error()) { <div class="error">{{ error() }}</div> }
      <p style="margin-top:1rem;font-size:.85rem;color:#6c757d;">
        A senha padrão de novos entregadores é <code>motoboy-default-password</code>.
      </p>
    </section>
  `
})
export class MotoboyLoginComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  cnpj = '';
  password = 'motoboy-default-password';
  protected loading = signal<boolean>(false);
  protected error = signal<string | null>(null);

  submit(ev: Event): void {
    ev.preventDefault();
    this.error.set(null);
    this.loading.set(true);
    this.auth.loginMotoboy(this.cnpj, this.password).subscribe({
      next: () => { this.loading.set(false); this.router.navigateByUrl('/motoboy/painel'); },
      error: (err: HttpErrorResponse) => { this.loading.set(false); this.error.set(extractErrorMessage(err)); }
    });
  }
}
