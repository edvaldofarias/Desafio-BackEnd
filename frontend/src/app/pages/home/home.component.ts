import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink],
  template: `
    <section class="card bg-gradient-to-br from-ink-900 to-ink-800 text-white border-0 ring-0">
      <div class="flex items-center gap-4">
        <img src="mottu-logo.svg" alt="Mottu" class="h-14 w-14 rounded-lg shadow-md" />
        <div>
          <p class="text-xs uppercase tracking-widest text-brand-200">Aplicação de teste</p>
          <h1 class="!text-white text-3xl mt-1">Mottu — Desafio Backend</h1>
        </div>
      </div>
      <p class="mt-4 text-slate-200 max-w-xl">
        Frontend de teste para o desafio. Selecione um perfil para entrar ou se cadastrar.
      </p>
    </section>

    <div class="grid gap-5 md:grid-cols-2">
      <article class="card flex flex-col gap-3 hover:shadow-md transition-shadow">
        <div class="flex items-center gap-3">
          <span class="text-2xl">🛠️</span>
          <h2>Admin</h2>
        </div>
        <p class="text-sm text-slate-600">Gerencie motos: cadastrar, listar, alterar placa e remover.</p>
        <div class="mt-auto pt-2">
          <a class="btn" routerLink="/admin/login">Entrar como admin</a>
        </div>
      </article>

      <article class="card flex flex-col gap-3 hover:shadow-md transition-shadow">
        <div class="flex items-center gap-3">
          <span class="text-2xl">🛵</span>
          <h2>Entregador</h2>
        </div>
        <p class="text-sm text-slate-600">Cadastre-se, envie sua CNH e alugue uma moto disponível.</p>
        <div class="mt-auto pt-2 flex gap-2">
          <a class="btn" routerLink="/motoboy/login">Entrar</a>
          <a class="btn btn-link" routerLink="/motoboy/cadastro">Cadastrar-se</a>
        </div>
      </article>
    </div>
  `
})
export class HomeComponent {}
