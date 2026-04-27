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
        <label for="nome">Nome</label>
        <input id="nome" name="nome" [(ngModel)]="form.nome" required />

        <label for="cnpj">CNPJ</label>
        <input id="cnpj" name="cnpj" [(ngModel)]="form.cnpj" required />

        <label for="senha">Senha</label>
        <input id="senha" name="senha" type="password" minlength="6"
               [(ngModel)]="form.senha" required />

        <label for="senha_confirma">Confirmar senha</label>
        <input id="senha_confirma" name="senha_confirma" type="password" minlength="6"
               [(ngModel)]="confirmPassword" required />

        <label for="data_nascimento">Data de nascimento</label>
        <input id="data_nascimento" name="data_nascimento" type="date"
               [max]="maxBirthDate" [(ngModel)]="form.data_nascimento" required />

        <label for="numero_cnh">Número da CNH</label>
        <input id="numero_cnh" name="numero_cnh" [(ngModel)]="form.numero_cnh" required />

        <label for="tipo_cnh">Tipo da CNH</label>
        <select id="tipo_cnh" name="tipo_cnh" [(ngModel)]="form.tipo_cnh" required>
          <option value="A">A (Moto)</option>
          <option value="B">B (Carro)</option>
          <option value="AB">A+B (Moto e Carro)</option>
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

  protected readonly maxBirthDate: string = this.isoDateYearsAgo(18);

  form: Omit<CreateMotoboy, 'identificador'> = {
    nome: '',
    cnpj: '',
    senha: '',
    data_nascimento: '',
    numero_cnh: '',
    tipo_cnh: 'A'
  };
  confirmPassword = '';

  private isoDateYearsAgo(years: number): string {
    const d = new Date();
    d.setFullYear(d.getFullYear() - years);
    const yyyy = d.getFullYear();
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const dd = String(d.getDate()).padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`;
  }

  submit(ev: Event): void {
    ev.preventDefault();
    this.error.set(null);
    this.success.set(false);
    if (this.form.senha !== this.confirmPassword) {
      this.error.set('As senhas não coincidem.');
      return;
    }
    this.loading.set(true);
    const payload: CreateMotoboy = {
      ...this.form,
      data_nascimento: new Date(this.form.data_nascimento).toISOString()
    };
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
