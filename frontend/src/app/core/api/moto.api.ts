import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Moto {
  ano: number;
  modelo: string;
  placa: string;
  identificador?: string;
}

export interface MotoResponse {
  identificador: string;
  ano: number;
  modelo: string;
  placa: string;
}

@Injectable({ providedIn: 'root' })
export class MotoApi {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/motos`;

  list(placa?: string): Observable<MotoResponse[]> {
    const qs = placa ? `?placa=${encodeURIComponent(placa)}` : '';
    return this.http.get<MotoResponse[]>(`${this.base}${qs}`);
  }
  create(body: Moto): Observable<unknown> { return this.http.post(this.base, body); }
  updatePlate(id: string, placa: string): Observable<unknown> {
    return this.http.put(`${this.base}/${id}/placa`, { placa });
  }
  remove(id: string): Observable<unknown> { return this.http.delete(`${this.base}/${id}`); }
}
