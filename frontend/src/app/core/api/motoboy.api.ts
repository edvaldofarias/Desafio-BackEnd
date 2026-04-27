import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface CreateMotoboy {
  identificador: string;
  nome: string;
  cnpj: string;
  data_nascimento: string;
  numero_cnh: string;
  tipo_cnh: 'A' | 'B' | 'AB';
  imagem_cnh?: string;
}

@Injectable({ providedIn: 'root' })
export class MotoboyApi {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/entregadores`;

  register(body: CreateMotoboy): Observable<unknown> { return this.http.post(this.base, body); }
  uploadCnh(id: string, base64: string): Observable<unknown> {
    return this.http.post(`${this.base}/${id}/cnh`, { imagem_cnh: base64 });
  }
}
