import { HttpErrorResponse } from '@angular/common/http';

interface ApiErrorBody {
  mensagem?: string;
  message?: string;
  errors?: Record<string, string[]>;
  title?: string;
}

function stripHtml(value: string): string {
  return value
    .replace(/<script[^>]*>[\s\S]*?<\/script>/gi, ' ')
    .replace(/<style[^>]*>[\s\S]*?<\/style>/gi, ' ')
    .replace(/<[^>]+>/g, ' ')
    .replace(/\s+/g, ' ')
    .trim();
}

function fallbackMessageByStatus(status: number): string {
  if (status === 0) return 'Nao foi possivel conectar ao servidor. Tente novamente.';
  if (status === 400) return 'Dados invalidos. Revise e tente novamente.';
  if (status === 401) return 'Sessao expirada. Faca login novamente.';
  if (status === 403) return 'Voce nao tem permissao para esta operacao.';
  if (status === 404) return 'Recurso nao encontrado.';
  if (status === 413) return 'Arquivo muito grande. O limite permitido e de 1 MB.';
  if (status >= 500) return 'Erro interno do servidor. Tente novamente em instantes.';
  return 'Erro inesperado';
}

export function extractErrorMessage(err: HttpErrorResponse): string {
  const body = err.error as ApiErrorBody | string | null | undefined;
  if (err.status === 0) return fallbackMessageByStatus(err.status);
  if (err.status === 413) return fallbackMessageByStatus(err.status);

  if (typeof body === 'string' && body.length > 0) {
    const asText = stripHtml(body);
    if (asText.length > 0 && !/request entity too large/i.test(asText)) return asText;
    return fallbackMessageByStatus(err.status);
  }

  if (body && typeof body === 'object') {
    if (body.mensagem) return body.mensagem;
    if (body.message) return body.message;
    if (body.errors) {
      const flat = Object.values(body.errors).flat();
      if (flat.length > 0) return flat.join(' · ');
    }
    if (body.title) return body.title;
  }

  if (err.message) {
    const asText = stripHtml(err.message);
    if (asText.length > 0 && !/request entity too large/i.test(asText)) return asText;
  }

  return fallbackMessageByStatus(err.status);
}
