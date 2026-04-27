import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export type Plan = 7 | 15 | 30 | 45 | 50;

export interface CreateRental {
  identificador: string;
  entregador_id: string;
  moto_id: string;
  data_inicio: string;
  data_termino: string;
  data_previsao_termino: string;
  plano: Plan;
}

export interface Rental {
  identificador: string;
  valor_diaria: number;
  entregador_id: string;
  moto_id: string;
  data_inicio: string;
  data_termino: string;
  data_previsao_termino: string;
  data_devolucao?: string;
  valor_total?: number;
  multa?: number;
}

@Injectable({ providedIn: 'root' })
export class RentalApi {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/locacao`;

  create(body: CreateRental): Observable<unknown> { return this.http.post(this.base, body); }
  get(id: string): Observable<Rental> { return this.http.get<Rental>(`${this.base}/${id}`); }
  return(id: string, dataDevolucao: string): Observable<unknown> {
    return this.http.put(`${this.base}/${id}/devolucao`, { data_devolucao: dataDevolucao });
  }
}
