import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { CreateMotoboy, MotoboyApi } from './motoboy.api';
import { environment } from '../../../environments/environment';

describe('MotoboyApi', () => {
  let api: MotoboyApi;
  let http: HttpTestingController;
  const base = `${environment.apiUrl}/entregadores`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    api = TestBed.inject(MotoboyApi);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('POST register with full payload', () => {
    const body: CreateMotoboy = {
      identificador: 'mb1', nome: 'Edvaldo', cnpj: '00000000000000',
      data_nascimento: '1990-01-01', numero_cnh: '12345678900', tipo_cnh: 'A'
    };
    api.register(body).subscribe();
    const req = http.expectOne(base);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(body);
    req.flush({});
  });

  it('POST upload CNH at /{id}/cnh', () => {
    api.uploadCnh('mb1', 'BASE64==').subscribe();
    const req = http.expectOne(`${base}/mb1/cnh`);
    expect(req.request.body).toEqual({ imagem_cnh: 'BASE64==' });
    req.flush({});
  });
});
