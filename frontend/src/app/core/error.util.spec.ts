import { HttpErrorResponse } from '@angular/common/http';
import { extractErrorMessage } from './error.util';

describe('extractErrorMessage', () => {
  it('reads `mensagem`', () => {
    const e = new HttpErrorResponse({ error: { mensagem: 'boom' }, status: 400 });
    expect(extractErrorMessage(e)).toBe('boom');
  });

  it('reads `message` fallback', () => {
    const e = new HttpErrorResponse({ error: { message: 'oops' }, status: 400 });
    expect(extractErrorMessage(e)).toBe('oops');
  });

  it('flattens validation `errors` map', () => {
    const e = new HttpErrorResponse({ error: { errors: { email: ['invalido'], senha: ['curta'] } }, status: 422 });
    expect(extractErrorMessage(e)).toBe('invalido · curta');
  });

  it('uses string body when error is a plain string', () => {
    const e = new HttpErrorResponse({ error: 'erro plano', status: 500 });
    expect(extractErrorMessage(e)).toBe('erro plano');
  });

  it('returns friendly message for 413 and html response body', () => {
    const html = '<html><head><title>413 Request Entity Too Large</title></head><body><h1>413 Request Entity Too Large</h1></body></html>';
    const e = new HttpErrorResponse({ error: html, status: 413 });
    expect(extractErrorMessage(e)).toContain('Arquivo muito grande');
  });

  it('strips html tags from generic string response', () => {
    const e = new HttpErrorResponse({ error: '<p>Falha ao processar envio</p>', status: 400 });
    expect(extractErrorMessage(e)).toBe('Falha ao processar envio');
  });

  it('falls back to err.message', () => {
    const e = new HttpErrorResponse({ error: null, status: 0 });
    expect(extractErrorMessage(e)).toContain('Nao foi possivel conectar');
  });
});
