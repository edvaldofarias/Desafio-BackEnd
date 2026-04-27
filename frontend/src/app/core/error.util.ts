import { HttpErrorResponse } from '@angular/common/http';

interface ApiErrorBody {
  mensagem?: string;
  message?: string;
  errors?: Record<string, string[]>;
  title?: string;
}

export function extractErrorMessage(err: HttpErrorResponse): string {
  const body = err.error as ApiErrorBody | string | null | undefined;
  if (typeof body === 'string' && body.length > 0) return body;
  if (body && typeof body === 'object') {
    if (body.mensagem) return body.mensagem;
    if (body.message) return body.message;
    if (body.errors) {
      const flat = Object.values(body.errors).flat();
      if (flat.length > 0) return flat.join(' · ');
    }
    if (body.title) return body.title;
  }
  return err.message || 'Erro inesperado';
}
