import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { MotoApi, MotoResponse } from '../../core/api/moto.api';
import { CreateRental, Plan, Rental, RentalApi } from '../../core/api/rental.api';
import { extractErrorMessage } from '../../core/error.util';

const CNH_UPDATE_STATUS_KEY = 'mottu.cnhUpdateStatus';

@Component({
  selector: 'app-motoboy-dashboard',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  template: `
    <h1>Painel do Entregador</h1>
    @if (toastMessage(); as msg) {
      <div style="position:fixed;top:1rem;right:1rem;z-index:60;max-width:24rem;" class="success">
        {{ msg }}
      </div>
    }

    <section class="card">
      <h2>Atualizacao cadastral</h2>
      @if (!cnhUpdateOk()) {
        <p style="margin: .4rem 0 1rem; color: rgb(71 85 105);">Voce ainda precisa fazer a atualizacao cadastral, enviando a foto da CNH.</p>
      }
      <a routerLink="/motoboy/atualizacao-cadastral" class="btn">Ir para atualizacao cadastral</a>
    </section>

    <section class="card">
      <h2>Alugar moto</h2>
      <form (submit)="createRental($event)" class="row">
        <div>
          <label for="motoid">Moto disponivel</label>
          <select id="motoid" name="moto_id" [(ngModel)]="rental.moto_id" [disabled]="loadingMotos() || motosDisponiveis().length === 0" required>
            @if (loadingMotos()) {
              <option value="">Carregando motos...</option>
            } @else {
              @for (moto of motosDisponiveis(); track moto.identificador) {
                <option [ngValue]="moto.identificador">{{ moto.modelo }} - {{ moto.placa }} ({{ moto.identificador }})</option>
              }
            }
          </select>
          @if (!loadingMotos() && motosDisponiveis().length === 0 && !motosError()) {
            <p style="margin:.35rem 0 0; color: rgb(100 116 139); font-size: .82rem;">Nenhuma moto disponivel para aluguel no momento.</p>
          }
          @if (motosError()) {
            <div class="error" style="margin-top:.5rem;">Nao foi possivel carregar as motos disponiveis: {{ motosError() }}</div>
            <button class="btn btn-secondary" type="button" style="margin-top:.5rem;" (click)="reloadMotos()">Tentar novamente</button>
          }
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
          <button class="btn" type="submit" [disabled]="creatingRental() || loadingMotos() || motosDisponiveis().length === 0 || !rental.moto_id">Alugar</button>
        </div>
      </form>
      @if (rentalError()) { <div class="error">{{ rentalError() }}</div> }
    </section>

    <section class="card">
      <div style="display:flex;justify-content:space-between;align-items:center;gap:.75rem;flex-wrap:wrap;">
        <h2 style="margin:0;">Minhas locações</h2>
      </div>

      @if (loadingRentals()) {
        <p style="margin-top:1rem;color:rgb(100 116 139);">Carregando locações...</p>
      } @else if (rentals().length) {
        <table style="margin-top:1rem;">
          <thead>
            <tr>
              <th>Identificador</th>
              <th>Moto</th>
              <th>Início</th>
              <th>Previsão</th>
              <th>Status</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            @for (rent of rentals(); track rent.identificador) {
              <tr>
                <td>{{ rent.identificador }}</td>
                <td>{{ formatMoto(rent) }}</td>
                <td>{{ rent.data_inicio | date:'dd/MM/yyyy' }}</td>
                <td>{{ rent.data_previsao_termino | date:'dd/MM/yyyy' }}</td>
                <td>{{ rent.data_devolucao ? 'Devolvida' : 'Em andamento' }}</td>
                <td>
                  <div style="display:flex;gap:.35rem;flex-wrap:wrap;">
                    <button class="btn btn-secondary" type="button" (click)="consultRental(rent.identificador)">Consultar</button>
                    @if (!rent.data_devolucao) {
                      <button class="btn" type="button" [disabled]="returning()" (click)="returnRental(rent.identificador)">Devolver</button>
                    }
                  </div>
                </td>
              </tr>
            }
          </tbody>
        </table>
      } @else {
        <p style="margin-top:1rem;color:rgb(100 116 139);">Nenhuma locação encontrada.</p>
      }

      @if (current(); as r) {
        <h3 style="margin-top:1.2rem;margin-bottom:.3rem;font-size:1rem;color:rgb(51 65 85);">Detalhes da locação</h3>
        <table style="margin-top:1rem;">
          <tbody>
            <tr><th>Início</th><td>{{ r.data_inicio | date:'dd/MM/yyyy' }}</td></tr>
            <tr><th>Previsão de término</th><td>{{ r.data_previsao_termino | date:'dd/MM/yyyy' }}</td></tr>
            <tr><th>Término</th><td>{{ r.data_termino | date:'dd/MM/yyyy' }}</td></tr>
            <tr><th>Valor diária</th><td>{{ r.valor_diaria | currency:'BRL' }}</td></tr>
            @if (r.valor_total !== undefined && r.valor_total !== null) { <tr><th>Total</th><td>{{ r.valor_total | currency:'BRL' }}</td></tr> }
            @if (r.multa !== undefined && r.multa !== null) { <tr><th>Multa</th><td>{{ r.multa | currency:'BRL' }}</td></tr> }
          </tbody>
        </table>
      }
      @if (rentalsError()) { <div class="error">{{ rentalsError() }}</div> }
      @if (lookupError()) { <div class="error">{{ lookupError() }}</div> }
    </section>
  `
})
export class MotoboyDashboardComponent implements OnInit {
  private auth = inject(AuthService);
  private motoApi = inject(MotoApi);
  private rentalApi = inject(RentalApi);

  protected cnhUpdateOk = signal<boolean>(this.hasCnhUpdateOk());
  protected motosDisponiveis = signal<MotoResponse[]>([]);
  protected loadingMotos = signal<boolean>(false);
  protected motosError = signal<string | null>(null);

  rental: CreateRental = this.emptyRental();
  protected creatingRental = signal<boolean>(false);
  protected rentalError = signal<string | null>(null);
  protected toastMessage = signal<string | null>(null);

  lookupId = '';
  protected rentals = signal<Rental[]>([]);
  protected loadingRentals = signal<boolean>(false);
  protected rentalsError = signal<string | null>(null);
  protected current = signal<Rental | null>(null);
  protected lookupError = signal<string | null>(null);

  protected returning = signal<boolean>(false);

  ngOnInit(): void {
    this.loadMotosDisponiveis();
    this.loadRentals();
  }

  createRental(ev: Event): void {
    ev.preventDefault();
    const motoboyId = this.auth.motoboyId();
    if (!motoboyId) return;
    this.rentalError.set(null); this.creatingRental.set(true);

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
      next: created => {
        this.creatingRental.set(false);
        this.lookupId = created?.identificador ?? '';
        this.showToast('Locacao criada com sucesso.');
        this.loadRentals();
        this.resetRentalForm();
      },
      error: (err: HttpErrorResponse) => { this.creatingRental.set(false); this.rentalError.set(extractErrorMessage(err)); }
    });
  }

  consultRental(id: string): void {
    this.lookupId = id;
    this.loadRental();
  }

  loadRental(): void {
    if (!this.lookupId) return;
    this.lookupError.set(null);
    this.rentalApi.get(this.lookupId).subscribe({
      next: r => this.current.set(r),
      error: (err: HttpErrorResponse) => { this.current.set(null); this.lookupError.set(extractErrorMessage(err)); }
    });
  }

  returnRental(id: string): void {
    if (!confirm('Confirmar devolucao desta locacao com a data de hoje?')) return;
    this.returning.set(true);
    const iso = new Date().toISOString();
    this.rentalApi.return(id, iso).subscribe({
      next: () => {
        this.returning.set(false);
        this.showToast('Locacao devolvida com sucesso.');
        this.loadRentals();
        if (this.lookupId === id) this.loadRental();
      },
      error: (err: HttpErrorResponse) => { this.returning.set(false); this.lookupError.set(extractErrorMessage(err)); }
    });
  }

  reloadMotos(): void {
    this.loadMotosDisponiveis();
  }

  private emptyRental(): CreateRental {
    return {
      entregador_id: '',
      moto_id: '',
      data_inicio: '',
      data_termino: '',
      data_previsao_termino: '',
      plano: 7 satisfies Plan
    };
  }

  protected formatMoto(rent: Rental): string {
    if (rent.moto_modelo && rent.moto_placa) return `${rent.moto_modelo} - ${rent.moto_placa}`;
    return rent.moto_id;
  }

  private loadMotosDisponiveis(): void {
    this.loadingMotos.set(true);
    this.motosError.set(null);

    this.motoApi.list().subscribe({
      next: motos => {
        this.loadingMotos.set(false);
        this.motosDisponiveis.set(motos);
        if (motos.length > 0 && !motos.some(m => m.identificador === this.rental.moto_id)) {
          this.rental.moto_id = motos[0].identificador;
        }
      },
      error: (err: HttpErrorResponse) => {
        this.loadingMotos.set(false);
        this.motosDisponiveis.set([]);
        this.motosError.set(extractErrorMessage(err));
      }
    });
  }

  private resetRentalForm(): void {
    const defaultMotoId = this.motosDisponiveis()[0]?.identificador ?? '';
    this.rental = {
      ...this.emptyRental(),
      moto_id: defaultMotoId,
      plano: 7
    };
  }

  private loadRentals(): void {
    const motoboyId = this.auth.motoboyId();
    if (!motoboyId) return;

    this.loadingRentals.set(true);
    this.rentalsError.set(null);

    this.rentalApi.listByMotoboy(motoboyId).subscribe({
      next: rentals => {
        this.loadingRentals.set(false);
        this.rentals.set(rentals);
      },
      error: (err: HttpErrorResponse) => {
        this.loadingRentals.set(false);
        this.rentals.set([]);
        this.rentalsError.set(extractErrorMessage(err));
      }
    });
  }

  private showToast(message: string): void {
    this.toastMessage.set(message);
    setTimeout(() => {
      if (this.toastMessage() === message) this.toastMessage.set(null);
    }, 2800);
  }

  private hasCnhUpdateOk(): boolean {
    return localStorage.getItem(CNH_UPDATE_STATUS_KEY) === 'ok';
  }
}
