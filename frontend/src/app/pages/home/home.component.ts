import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink],
  template: `
    <section class="card hero">
      <h1>Mottu — Aplicação de Teste</h1>
      <p>
        Esta é uma aplicação <strong>de teste</strong> que consome a API do desafio
        backend Mottu. Selecione um perfil para entrar ou se cadastrar.
      </p>
    </section>

    <div class="profiles">
      <article class="card profile">
        <h2>Admin</h2>
        <p>Gerencie motos: cadastrar, listar, alterar placa e remover.</p>
        <a class="btn" routerLink="/admin/login">Entrar como admin</a>
      </article>

      <article class="card profile">
        <h2>Entregador</h2>
        <p>Cadastre-se, envie sua CNH e alugue uma moto disponível.</p>
        <a class="btn" routerLink="/motoboy/login">Entrar como entregador</a>
        <a class="btn btn-link" routerLink="/motoboy/cadastro">ou cadastre-se</a>
      </article>
    </div>
  `,
  styles: [`
    .hero { background: linear-gradient(135deg, #0d2540, #1976d2); color: #fff; }
    .hero h1 { margin: 0 0 0.6rem; }
    .profiles { display: grid; grid-template-columns: 1fr 1fr; gap: 1.25rem; }
    .profile { display: flex; flex-direction: column; gap: 0.5rem; align-items: flex-start; }
    .profile h2 { margin: 0; }
    @media (max-width: 640px) { .profiles { grid-template-columns: 1fr; } }
  `]
})
export class HomeComponent {}
