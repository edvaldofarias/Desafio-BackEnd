import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { CreateMotoboy, MotoboyApi } from '../../core/api/motoboy.api';
import { extractErrorMessage } from '../../core/error.util';

@Component({
  selector: 'app-motoboy-register',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    <section class="card" style="max-width:560px;margin:0 auto;">
      <h1>Cadastro de Entregador</h1>
      <form (submit)="submit($event)">
        <label for="identificador">Identificador</label>
        <input id="identificador" name="identificador" [(ngModel)]="form.identificador" required />

        <label for="nome">Nome</label>
        <input id="nome" name="nome" [(ngModel)]="form.nome" required />

        <label for="cnpj">CNPJ</label>
        <input id="cnpj" name="cnpj" [(ngModel)]="form.cnpj" required />

        <label for="data_nascimento">Data de nascimento</label>
        <input id="data_nascimento" name="data_nascimento" type="date" [(ngModel)]="form.data_nascimento" required />

        <label for="numero_cnh">Número da CNH</label>
        <input id="numero_cnh" name="numero_cnh" [(ngModel)]="form.numero_cnh" required />

        <label for="tipo_cnh">Tipo da CNH</label>
        <select id="tipo_cnh" name="tipo_cnh" [(ngModel)]="form.tipo_cnh" required>
          <option value="A">A</option>
          <option value="B">B</option>
          <option value="AB">A+B</option>
        </select>

        <div style="margin-top:1.2rem;display:flex;gap:.6rem;">
          <button class="btn" type="submit" [disabled]="loading()">
            {{ loading() ? 'Enviando…' : 'Cadastrar' }}
          </button>
          <a class="btn btn-link" routerLink="/motoboy/login">Já tenho cadastro</a>
        </div>
      </form>
      @if (error()) { <div class="error">{{ error() }}</div> }
      @if (success()) { <div class="success">Cadastro realizado. Faça login agora.</div> }
    </section>
  `
})
export class MotoboyRegisterComponent {
  private api = inject(MotoboyApi);
  private router = inject(Router);

  protected loading = signal<boolean>(false);
  protected error = signal<string | null>(null);
  protected success = signal<boolean>(false);

  form: CreateMotoboy = {
    identificador: '',
    nome: '',
    cnpj: '',
    data_nascimento: '',
    numero_cnh: '',
    tipo_cnh: 'A'
  };

  submit(ev: Event): void {
    ev.preventDefault();
    this.error.set(null);
    this.success.set(false);
    this.loading.set(true);
    const payload: CreateMotoboy = { ...this.form, data_nascimento: new Date(this.form.data_nascimento).toISOString() };
    this.api.register(payload).subscribe({
      next: () => {
        this.loading.set(false);
        this.success.set(true);
        setTimeout(() => this.router.navigateByUrl('/motoboy/login'), 1200);
      },
      error: (err: HttpErrorResponse) => { this.loading.set(false); this.error.set(extractErrorMessage(err)); }
    });
  }
}
