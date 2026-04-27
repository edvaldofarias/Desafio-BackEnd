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

  it('falls back to err.message', () => {
    const e = new HttpErrorResponse({ error: null, status: 0 });
    expect(extractErrorMessage(e)).toContain('Http failure');
  });
});
