import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Moto, MotoApi } from './moto.api';
import { environment } from '../../../environments/environment';

describe('MotoApi', () => {
  let api: MotoApi;
  let http: HttpTestingController;
  const base = `${environment.apiUrl}/motos`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    api = TestBed.inject(MotoApi);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('GET list with no filter', () => {
    api.list().subscribe();
    const req = http.expectOne(base);
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('GET list with placa filter URL-encoded', () => {
    api.list('ABC 1D23').subscribe();
    const req = http.expectOne(`${base}?placa=ABC%201D23`);
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('POST create sends body', () => {
    const moto: Moto = { ano: 2024, modelo: 'CG', placa: 'ABC1D23' };
    api.create(moto).subscribe();
    const req = http.expectOne(base);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(moto);
    req.flush({});
  });

  it('PUT updatePlate uses placa key', () => {
    api.updatePlate('m1', 'XYZ1A23').subscribe();
    const req = http.expectOne(`${base}/m1/placa`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual({ placa: 'XYZ1A23' });
    req.flush({});
  });

  it('DELETE removes by id', () => {
    api.remove('m1').subscribe();
    const req = http.expectOne(`${base}/m1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({});
  });
});
