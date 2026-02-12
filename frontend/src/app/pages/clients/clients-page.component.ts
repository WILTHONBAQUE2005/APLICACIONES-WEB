import { Component, OnInit, inject } from '@angular/core';
import { AsyncPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ClientService } from '../../features/clients/client.service';
import { ToastService } from '../../shared/toast.service';
import type { ClientListItem, ClientUpsert } from '../../core/http/types';

@Component({
  standalone: true,
  imports: [NgIf, NgFor, ReactiveFormsModule, DatePipe, AsyncPipe],
  templateUrl: './clients-page.component.html'
})
export class ClientsPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ClientService);
  private readonly toasts = inject(ToastService);

  loading = false;
  saving = false;

  items: ClientListItem[] = [];
  search = this.fb.control('');

  modalOpen = false;
  editing: ClientListItem | null = null;

  form = this.fb.group({
    documentType: ['CEDULA', [Validators.required]],
    documentNumber: ['', [Validators.required, Validators.minLength(5)]],
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.email]],
    phone: [''],
    addressLine1: [''],
    city: [''],
    notes: [''],
    isActive: [true, [Validators.required]]
  });

  get filtered(): ClientListItem[] {
    const q = (this.search.value || '').trim().toLowerCase();
    if (!q) return this.items;
    return this.items.filter(x =>
      (x.documentNumber || '').toLowerCase().includes(q) ||
      (x.firstName || '').toLowerCase().includes(q) ||
      (x.lastName || '').toLowerCase().includes(q) ||
      (x.email || '').toLowerCase().includes(q) ||
      (x.phone || '').toLowerCase().includes(q)
    );
  }

  async ngOnInit() {
    await this.reload();
  }

  async reload() {
    this.loading = true;
    try {
      this.items = await this.api.list();
    } finally {
      this.loading = false;
    }
  }

  openCreate() {
    this.editing = null;
    this.form.reset({
      documentType: 'CEDULA',
      documentNumber: '',
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      addressLine1: '',
      city: '',
      notes: '',
      isActive: true
    });
    this.modalOpen = true;
  }

  openEdit(item: ClientListItem) {
    this.editing = item;
    this.form.reset({
      documentType: item.documentType || 'CEDULA',
      documentNumber: item.documentNumber || '',
      firstName: item.firstName || '',
      lastName: item.lastName || '',
      email: item.email || '',
      phone: item.phone || '',
      addressLine1: item.addressLine1 || '',
      city: item.city || '',
      notes: item.notes || '',
      isActive: item.isActive
    });
    this.modalOpen = true;
  }

  closeModal() {
    if (this.saving) return;
    this.modalOpen = false;
  }

  async save() {
    if (this.form.invalid || this.saving) return;
    this.saving = true;

    const payload: ClientUpsert = {
      documentType: this.form.value.documentType || 'CEDULA',
      documentNumber: this.form.value.documentNumber || '',
      firstName: this.form.value.firstName || '',
      lastName: this.form.value.lastName || '',
      email: this.form.value.email || '',
      phone: this.form.value.phone || '',
      addressLine1: this.form.value.addressLine1 || '',
      city: this.form.value.city || '',
      notes: this.form.value.notes || '',
      isActive: !!this.form.value.isActive
    };

    try {
      if (this.editing) {
        await this.api.update(this.editing.id, payload);
        this.toasts.success('Cliente actualizado');
      } else {
        await this.api.create(payload);
        this.toasts.success('Cliente creado');
      }
      this.modalOpen = false;
      await this.reload();
    } finally {
      this.saving = false;
    }
  }

  async deactivate(item: ClientListItem) {
    const ok = confirm(`Desactivar a ${item.firstName} ${item.lastName}?`);
    if (!ok) return;
    await this.api.remove(item.id);
    this.toasts.info('Cliente desactivado');
    await this.reload();
  }

async toggleActive(item: ClientListItem) {
  const willActivate = !item.isActive;
  const action = willActivate ? 'Activar' : 'Desactivar';

  const ok = confirm(`${action} a ${item.firstName} ${item.lastName}?`);
  if (!ok) return;

  if (willActivate) {
    await this.api.activate(item.id);
    this.toasts.success('Cliente activado');
  } else {
    await this.api.remove(item.id);
    this.toasts.info('Cliente desactivado');
  }

  await this.reload();
}

}
