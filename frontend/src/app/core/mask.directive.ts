import { Directive, ElementRef, HostListener, Input, OnInit, inject } from '@angular/core';
import { NgControl } from '@angular/forms';
import { formatCnh, formatCnpj } from './mask.util';

export type MaskKind = 'cnpj' | 'cnh';

@Directive({
  selector: '[appMask]',
  standalone: true
})
export class MaskDirective implements OnInit {
  private readonly el = inject<ElementRef<HTMLInputElement>>(ElementRef);
  private readonly ngControl = inject(NgControl, { optional: true });

  @Input('appMask') kind: MaskKind = 'cnpj';

  private format(value: string): string {
    return this.kind === 'cnh' ? formatCnh(value) : formatCnpj(value);
  }

  ngOnInit(): void {
    const initial = this.el.nativeElement.value;
    if (initial) this.apply(initial);
  }

  @HostListener('input', ['$event'])
  onInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.apply(input.value);
  }

  private apply(value: string): void {
    const formatted = this.format(value);
    this.el.nativeElement.value = formatted;
    this.ngControl?.control?.setValue(formatted, { emitEvent: false });
  }
}
