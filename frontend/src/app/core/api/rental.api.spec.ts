import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { CreateRental, Rental, RentalApi } from './rental.api';
import { environment } from '../../../environments/environment';

describe('RentalApi', () => {
  let api: RentalApi;
  let http: HttpTestingController;
  const base = `${environment.apiUrl}/locacao`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    api = TestBed.inject(RentalApi);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('POST creates rental', () => {
    const body: CreateRental = {
      entregador_id: 'mb1', moto_id: 'm1',
      data_inicio: '2026-01-01', data_termino: '2026-01-08',
      data_previsao_termino: '2026-01-08', plano: 7
    };
    api.create(body).subscribe();
    const req = http.expectOne(base);
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('GET retrieves by id', () => {
    const fake: Rental = {
      identificador: 'r1', valor_diaria: 30, entregador_id: 'mb1', moto_id: 'm1',
      data_inicio: '', data_termino: '', data_previsao_termino: ''
    };
    api.get('r1').subscribe(r => expect(r.valor_diaria).toBe(30));
    const req = http.expectOne(`${base}/r1`);
    req.flush(fake);
  });

  it('PUT return uses snake_case payload', () => {
    api.return('r1', '2026-01-05T00:00:00Z').subscribe();
    const req = http.expectOne(`${base}/r1/devolucao`);
    expect(req.request.body).toEqual({ data_devolucao: '2026-01-05T00:00:00Z' });
    req.flush({});
  });
});
