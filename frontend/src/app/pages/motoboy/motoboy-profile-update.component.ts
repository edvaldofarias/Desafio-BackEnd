import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { MotoboyApi } from '../../core/api/motoboy.api';
import { extractErrorMessage } from '../../core/error.util';

const MAX_UPLOAD_BYTES = 1024 * 1024;
const CNH_UPDATE_STATUS_KEY = 'mottu.cnhUpdateStatus';

@Component({
  selector: 'app-motoboy-profile-update',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="row" style="align-items: center; margin-bottom: 1rem;">
      <div>
        <h1>Atualizacao cadastral</h1>
        <p class="hint">Atualize sua documentacao e acompanhe o status do envio da CNH.</p>
      </div>
      <div style="display:flex;justify-content:flex-end;align-items:center;">
        <a routerLink="/motoboy/painel" class="btn btn-secondary">Voltar ao painel</a>
      </div>
    </div>

    <section class="card">
      <h2>Enviar foto da CNH</h2>

      <label for="cnh-file">Arquivo (PNG/BMP/JPG/JPEG)</label>
      <div class="file-picker">
        <input
          id="cnh-file"
          class="native-file-input"
          type="file"
          accept="image/png,image/bmp,image/jpeg,image/jpg"
          [disabled]="cnhUploading()"
          (change)="onFileSelected($event)"
        />
        <label for="cnh-file" class="picker-button" [class.disabled]="cnhUploading()">Selecionar arquivo</label>
        <span class="file-name">{{ selectedFileName() || 'Nenhum arquivo selecionado' }}</span>
      </div>

      <div class="file-rules" aria-live="polite">
        <p class="hint">Requisitos do arquivo:</p>
        <ul>
          <li>Tipos permitidos: PNG, BMP, JPG ou JPEG</li>
          <li>Tamanho maximo permitido: 1 MB</li>
          <li>Imagem nitida, sem cortes e com boa iluminacao</li>
        </ul>
      </div>

      <button class="btn upload-submit" type="button" [disabled]="!cnhBase64 || cnhUploading()" (click)="uploadCnh()">
        @if (cnhUploading()) {
          <span class="spinner" aria-hidden="true"></span>
          Enviando...
        } @else {
          Publicar foto da CNH
        }
      </button>

      @if (cnhError(); as msg) {
        <div class="error" role="alert">
          <strong>Nao foi possivel enviar a CNH.</strong>
          <div>{{ msg }}</div>
        </div>
      }

      @if (cnhSuccess()) {
        <div class="success" role="status">Foto da CNH enviada com sucesso.</div>
      }
    </section>
  `,
  styles: [`
    .hint {
      margin: 0.6rem 0 0;
      color: rgb(71 85 105);
      font-size: 0.875rem;
    }

    .file-picker {
      margin-top: 0.5rem;
      display: flex;
      align-items: center;
      gap: 0.75rem;
      flex-wrap: wrap;
      padding: 0.85rem;
      border-radius: 0.8rem;
      border: 1px dashed rgb(148 163 184);
      background: rgb(248 250 252);
    }

    .native-file-input {
      position: absolute;
      width: 1px;
      height: 1px;
      overflow: hidden;
      clip: rect(0, 0, 0, 0);
      white-space: nowrap;
      clip-path: inset(50%);
      border: 0;
      margin: -1px;
      padding: 0;
    }

    .picker-button {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      border-radius: 0.7rem;
      border: 1px solid rgb(15 23 42 / 0.2);
      background: white;
      color: rgb(15 23 42);
      font-weight: 600;
      font-size: 0.85rem;
      text-transform: none;
      letter-spacing: 0;
      margin: 0;
      padding: 0.55rem 0.95rem;
      cursor: pointer;
      transition: all 0.15s ease;
    }

    .picker-button:hover {
      background: rgb(241 245 249);
      border-color: rgb(15 23 42 / 0.35);
    }

    .picker-button.disabled {
      pointer-events: none;
      opacity: 0.55;
    }

    .file-name {
      font-size: 0.88rem;
      color: rgb(51 65 85);
      font-weight: 500;
    }

    .file-rules {
      margin-top: 0.7rem;
      color: rgb(71 85 105);
    }

    .file-rules ul {
      margin: 0.35rem 0 0;
      padding-left: 1.1rem;
      font-size: 0.85rem;
      line-height: 1.4;
    }

    .upload-submit {
      margin-top: 1rem;
      background: linear-gradient(135deg, rgb(249 115 22), rgb(234 88 12));
      border: 1px solid rgb(194 65 12 / 0.55);
      box-shadow: 0 8px 18px -12px rgb(194 65 12 / 0.9);
    }

    .upload-submit:hover:not(:disabled) {
      filter: brightness(1.03);
      transform: translateY(-1px);
    }

    .upload-submit:active:not(:disabled) {
      transform: translateY(0);
    }

    .spinner {
      width: 0.95rem;
      height: 0.95rem;
      border-radius: 9999px;
      border: 2px solid rgba(255, 255, 255, 0.45);
      border-top-color: rgb(255, 255, 255);
      animation: spin 0.8s linear infinite;
      display: inline-block;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }
  `]
})
export class MotoboyProfileUpdateComponent {
  private auth = inject(AuthService);
  private motoboyApi = inject(MotoboyApi);

  protected cnhBase64: string | null = null;
  protected selectedFileName = signal<string | null>(null);
  protected cnhUploading = signal<boolean>(false);
  protected cnhError = signal<string | null>(null);
  protected cnhSuccess = signal<boolean>(false);

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;

    this.cnhSuccess.set(false);
    this.cnhError.set(null);

    if (!file) {
      this.cnhBase64 = null;
      this.selectedFileName.set(null);
      return;
    }

    const isAcceptedType = ['image/png', 'image/bmp', 'image/jpeg', 'image/jpg'].includes(file.type);
    if (!isAcceptedType) {
      this.cnhBase64 = null;
      this.selectedFileName.set(file.name);
      this.cnhError.set('Formato invalido. Envie um arquivo PNG, BMP, JPG ou JPEG.');
      return;
    }

    if (file.size > MAX_UPLOAD_BYTES) {
      this.cnhBase64 = null;
      this.selectedFileName.set(file.name);
      this.cnhError.set('Arquivo muito grande. O limite permitido e de 1 MB.');
      return;
    }

    this.selectedFileName.set(file.name);

    const reader = new FileReader();
    reader.onload = () => {
      const result = reader.result;
      if (typeof result === 'string') {
        const commaIndex = result.indexOf(',');
        this.cnhBase64 = commaIndex >= 0 ? result.slice(commaIndex + 1) : result;
      }
    };

    reader.onerror = () => {
      this.cnhBase64 = null;
      this.cnhError.set('Nao foi possivel ler o arquivo selecionado. Tente novamente.');
    };

    reader.readAsDataURL(file);
  }

  uploadCnh(): void {
    const motoboyId = this.auth.motoboyId();
    if (!motoboyId || !this.cnhBase64) {
      this.cnhError.set('Selecione um arquivo valido antes de enviar.');
      return;
    }

    this.cnhError.set(null);
    this.cnhSuccess.set(false);
    this.cnhUploading.set(true);

    this.motoboyApi.uploadCnh(motoboyId, this.cnhBase64).subscribe({
      next: () => {
        this.cnhUploading.set(false);
        this.cnhSuccess.set(true);
        localStorage.setItem(CNH_UPDATE_STATUS_KEY, 'ok');
      },
      error: (err: HttpErrorResponse) => {
        this.cnhUploading.set(false);
        this.cnhError.set(extractErrorMessage(err));
      }
    });
  }
}
