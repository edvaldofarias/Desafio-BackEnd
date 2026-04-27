import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/auth.service';
import { MotoboyApi } from '../../core/api/motoboy.api';
import { CreateRental, Plan, Rental, RentalApi } from '../../core/api/rental.api';
import { extractErrorMessage } from '../../core/error.util';

@Component({
  selector: 'app-motoboy-dashboard',
  standalone: true,
  imports: [FormsModule, CommonModule],
  template: `
    <h1>Painel do Entregador</h1>

    <section class="card">
      <h2>Enviar foto da CNH</h2>
      <label>
        Arquivo (PNG/BMP)
        <input type="file" accept="image/png,image/bmp" (change)="onFileSelected($event)" />
      </label>
      <button class="btn" type="button" [disabled]="!cnhBase64 || cnhUploading()" (click)="uploadCnh()" style="margin-left:.6rem;">
        {{ cnhUploading() ? 'Enviando…' : 'Enviar' }}
      </button>
      @if (cnhError()) { <div class="error">{{ cnhError() }}</div> }
      @if (cnhSuccess()) { <div class="success">CNH enviada com sucesso.</div> }
    </section>

    <section class="card">
      <h2>Alugar moto</h2>
      <form (submit)="createRental($event)" class="row">
        <div>
          <label for="rid">Identificador da locação</label>
          <input id="rid" name="identificador" [(ngModel)]="rental.identificador" required />
        </div>
        <div>
          <label for="motoid">ID da moto</label>
          <input id="motoid" name="moto_id" [(ngModel)]="rental.moto_id" required />
        </div>
        <div>
          <label for="plano">Plano (dias)</label>
          <select id="plano" name="plano" [(ngModel)]="rental.plano" required>
            <option [ngValue]="7">7</option>
            <option [ngValue]="15">15</option>
            <option [ngValue]="30">30</option>
            <option [ngValue]="45">45</option>
            <option [ngValue]="50">50</option>
          </select>
        </div>
        <div style="display:flex;align-items:flex-end;">
          <button class="btn" type="submit" [disabled]="creatingRental()">Alugar</button>
        </div>
      </form>
      @if (rentalError()) { <div class="error">{{ rentalError() }}</div> }
      @if (rentalCreated()) { <div class="success">Locação criada. Use o identificador acima para consultar.</div> }
    </section>

    <section class="card">
      <h2>Consultar / Devolver locação</h2>
      <div class="row">
        <input name="rentalId" placeholder="identificador da locação" [(ngModel)]="lookupId" />
        <button class="btn btn-secondary" type="button" (click)="loadRental()">Consultar</button>
      </div>

      @if (current(); as r) {
        <table style="margin-top:1rem;">
          <tbody>
            <tr><th>Início</th><td>{{ r.data_inicio | date:'shortDate' }}</td></tr>
            <tr><th>Previsão de término</th><td>{{ r.data_previsao_termino | date:'shortDate' }}</td></tr>
            <tr><th>Término</th><td>{{ r.data_termino | date:'shortDate' }}</td></tr>
            <tr><th>Valor diária</th><td>{{ r.valor_diaria | currency:'BRL' }}</td></tr>
            @if (r.valor_total !== undefined && r.valor_total !== null) { <tr><th>Total</th><td>{{ r.valor_total | currency:'BRL' }}</td></tr> }
            @if (r.multa !== undefined && r.multa !== null) { <tr><th>Multa</th><td>{{ r.multa | currency:'BRL' }}</td></tr> }
          </tbody>
        </table>

        <div style="margin-top:1rem;" class="row">
          <input type="date" name="ret" [(ngModel)]="returnDate" />
          <button class="btn" type="button" [disabled]="returning()" (click)="doReturn()">Informar devolução</button>
        </div>
      }
      @if (lookupError()) { <div class="error">{{ lookupError() }}</div> }
    </section>
  `
})
export class MotoboyDashboardComponent {
  private auth = inject(AuthService);
  private motoboyApi = inject(MotoboyApi);
  private rentalApi = inject(RentalApi);

  protected cnhBase64: string | null = null;
  protected cnhUploading = signal<boolean>(false);
  protected cnhError = signal<string | null>(null);
  protected cnhSuccess = signal<boolean>(false);

  rental: CreateRental = this.emptyRental();
  protected creatingRental = signal<boolean>(false);
  protected rentalError = signal<string | null>(null);
  protected rentalCreated = signal<boolean>(false);

  lookupId = '';
  protected current = signal<Rental | null>(null);
  protected lookupError = signal<string | null>(null);

  returnDate = '';
  protected returning = signal<boolean>(false);

  onFileSelected(ev: Event): void {
    const input = ev.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    if (!file) { this.cnhBase64 = null; return; }
    const reader = new FileReader();
    reader.onload = () => {
      const result = reader.result;
      if (typeof result === 'string') {
        const comma = result.indexOf(',');
        this.cnhBase64 = comma >= 0 ? result.slice(comma + 1) : result;
      }
    };
    reader.readAsDataURL(file);
  }

  uploadCnh(): void {
    const id = this.auth.motoboyId();
    if (!id || !this.cnhBase64) return;
    this.cnhError.set(null); this.cnhSuccess.set(false); this.cnhUploading.set(true);
    this.motoboyApi.uploadCnh(id, this.cnhBase64).subscribe({
      next: () => { this.cnhUploading.set(false); this.cnhSuccess.set(true); },
      error: (err: HttpErrorResponse) => { this.cnhUploading.set(false); this.cnhError.set(extractErrorMessage(err)); }
    });
  }

  createRental(ev: Event): void {
    ev.preventDefault();
    const motoboyId = this.auth.motoboyId();
    if (!motoboyId) return;
    this.rentalError.set(null); this.rentalCreated.set(false); this.creatingRental.set(true);

    const today = new Date();
    const start = new Date(today); start.setDate(start.getDate() + 1);
    const end = new Date(start); end.setDate(end.getDate() + this.rental.plano);

    const payload: CreateRental = {
      ...this.rental,
      entregador_id: motoboyId,
      data_inicio: start.toISOString(),
      data_termino: end.toISOString(),
      data_previsao_termino: end.toISOString()
    };

    this.rentalApi.create(payload).subscribe({
      next: () => {
        this.creatingRental.set(false);
        this.rentalCreated.set(true);
        this.lookupId = payload.identificador;
        this.rental = this.emptyRental();
      },
      error: (err: HttpErrorResponse) => { this.creatingRental.set(false); this.rentalError.set(extractErrorMessage(err)); }
    });
  }

  loadRental(): void {
    if (!this.lookupId) return;
    this.lookupError.set(null);
    this.rentalApi.get(this.lookupId).subscribe({
      next: r => this.current.set(r),
      error: (err: HttpErrorResponse) => { this.current.set(null); this.lookupError.set(extractErrorMessage(err)); }
    });
  }

  doReturn(): void {
    if (!this.lookupId || !this.returnDate) return;
    this.returning.set(true);
    const iso = new Date(this.returnDate).toISOString();
    this.rentalApi.return(this.lookupId, iso).subscribe({
      next: () => { this.returning.set(false); this.loadRental(); },
      error: (err: HttpErrorResponse) => { this.returning.set(false); this.lookupError.set(extractErrorMessage(err)); }
    });
  }

  private emptyRental(): CreateRental {
    return {
      identificador: '',
      entregador_id: '',
      moto_id: '',
      data_inicio: '',
      data_termino: '',
      data_previsao_termino: '',
      plano: 7 satisfies Plan
    };
  }
}
