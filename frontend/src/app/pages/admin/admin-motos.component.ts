import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Moto, MotoApi } from '../../core/api/moto.api';
import { extractErrorMessage } from '../../core/error.util';

@Component({
  selector: 'app-admin-motos',
  standalone: true,
  imports: [FormsModule],
  template: `
    <h1>Motos</h1>

    <section class="card">
      <h2>Cadastrar moto</h2>
      <form (submit)="create($event)" class="row">
        <div>
          <label for="ident">Identificador</label>
          <input id="ident" name="ident" [(ngModel)]="form.identificador" required />
        </div>
        <div>
          <label for="ano">Ano</label>
          <input id="ano" name="ano" type="number" [(ngModel)]="form.ano" required />
        </div>
        <div>
          <label for="modelo">Modelo</label>
          <input id="modelo" name="modelo" [(ngModel)]="form.modelo" required />
        </div>
        <div>
          <label for="placa">Placa</label>
          <input id="placa" name="placa" [(ngModel)]="form.placa" required />
        </div>
        <div style="display:flex;align-items:flex-end;">
          <button class="btn" type="submit" [disabled]="creating()">Adicionar</button>
        </div>
      </form>
      @if (createError()) { <div class="error">{{ createError() }}</div> }
      @if (createSuccess()) { <div class="success">Moto cadastrada.</div> }
    </section>

    <section class="card">
      <h2>Listar motos</h2>
      <div class="row">
        <input name="filter" placeholder="filtrar por placa" [(ngModel)]="filter" />
        <button class="btn btn-secondary" type="button" (click)="reload()">Recarregar</button>
      </div>

      @if (motos().length) {
        <table style="margin-top:1rem;">
          <thead><tr><th>Identificador</th><th>Ano</th><th>Modelo</th><th>Placa</th><th></th></tr></thead>
          <tbody>
            @for (m of motos(); track m.identificador) {
              <tr>
                <td>{{ m.identificador }}</td>
                <td>{{ m.ano }}</td>
                <td>{{ m.modelo }}</td>
                <td>
                  @if (editingId() === m.identificador) {
                    <input [(ngModel)]="newPlate" name="newPlate-{{m.identificador}}" />
                    <button class="btn btn-link" type="button" (click)="savePlate(m.identificador)">salvar</button>
                  } @else {
                    {{ m.placa }}
                    <button class="btn btn-link" type="button" (click)="startEdit(m)">editar</button>
                  }
                </td>
                <td>
                  <button class="btn btn-danger" type="button" (click)="remove(m.identificador)">remover</button>
                </td>
              </tr>
            }
          </tbody>
        </table>
      } @else {
        <p style="margin-top:1rem;color:#6c757d;">Nenhuma moto encontrada.</p>
      }
      @if (listError()) { <div class="error">{{ listError() }}</div> }
    </section>
  `
})
export class AdminMotosComponent implements OnInit {
  private api = inject(MotoApi);

  protected motos = signal<Moto[]>([]);
  protected creating = signal<boolean>(false);
  protected createError = signal<string | null>(null);
  protected createSuccess = signal<boolean>(false);
  protected listError = signal<string | null>(null);
  protected editingId = signal<string | null>(null);

  filter = '';
  newPlate = '';
  form: Moto = { identificador: '', ano: new Date().getFullYear(), modelo: '', placa: '' };

  ngOnInit(): void { this.reload(); }

  reload(): void {
    this.listError.set(null);
    this.api.list(this.filter || undefined).subscribe({
      next: list => this.motos.set(list),
      error: (err: HttpErrorResponse) => this.listError.set(extractErrorMessage(err))
    });
  }

  create(ev: Event): void {
    ev.preventDefault();
    this.createError.set(null);
    this.createSuccess.set(false);
    this.creating.set(true);
    this.api.create(this.form).subscribe({
      next: () => {
        this.creating.set(false);
        this.createSuccess.set(true);
        this.form = { identificador: '', ano: new Date().getFullYear(), modelo: '', placa: '' };
        this.reload();
      },
      error: (err: HttpErrorResponse) => { this.creating.set(false); this.createError.set(extractErrorMessage(err)); }
    });
  }

  startEdit(m: Moto): void { this.editingId.set(m.identificador); this.newPlate = m.placa; }
  savePlate(id: string): void {
    this.api.updatePlate(id, this.newPlate).subscribe({
      next: () => { this.editingId.set(null); this.reload(); },
      error: (err: HttpErrorResponse) => this.listError.set(extractErrorMessage(err))
    });
  }
  remove(id: string): void {
    if (!confirm('Remover esta moto?')) return;
    this.api.remove(id).subscribe({
      next: () => this.reload(),
      error: (err: HttpErrorResponse) => this.listError.set(extractErrorMessage(err))
    });
  }
}
