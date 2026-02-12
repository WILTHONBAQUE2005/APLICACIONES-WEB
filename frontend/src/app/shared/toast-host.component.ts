import { Component, inject } from '@angular/core';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-toast-host',
  standalone: true,
  imports: [NgFor, NgIf, AsyncPipe],
  templateUrl: './toast-host.component.html'
})
export class ToastHostComponent {
  private readonly service = inject(ToastService);
  readonly toasts = this.service.toasts$;
  dismiss(id: string) { this.service.dismiss(id); }
}
