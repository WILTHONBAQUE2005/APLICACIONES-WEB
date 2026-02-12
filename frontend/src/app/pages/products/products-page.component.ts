import { Component, OnInit, inject } from '@angular/core';
import { DatePipe, DecimalPipe, NgFor, NgIf } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductService } from '../../features/products/product.service';
import { ToastService } from '../../shared/toast.service';
import type { ProductListItem, ProductUpsert } from '../../core/http/types';

@Component({
  standalone: true,
  imports: [NgIf, NgFor, ReactiveFormsModule, DatePipe, DecimalPipe],
  templateUrl: './products-page.component.html'
})
export class ProductsPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(ProductService);
  private readonly toasts = inject(ToastService);

  loading = false;
  saving = false;

  items: ProductListItem[] = [];
  search = this.fb.control('');

  modalOpen = false;
  editing: ProductListItem | null = null;

  form = this.fb.group({
    sku: ['', [Validators.required, Validators.minLength(3)]],
    name: ['', [Validators.required, Validators.minLength(2)]],
    category: [''],
    description: [''],
    barcode: [''],
    imageUrl: [''],
    price: [0, [Validators.required, Validators.min(0)]],
    stock: [0, [Validators.required, Validators.min(0)]],
    isActive: [true, [Validators.required]]
  });

  get filtered(): ProductListItem[] {
    const q = (this.search.value || '').trim().toLowerCase();
    if (!q) return this.items;
    return this.items.filter(x =>
      (x.sku || '').toLowerCase().includes(q) ||
      (x.name || '').toLowerCase().includes(q) ||
      (x.category || '').toLowerCase().includes(q) ||
      (x.barcode || '').toLowerCase().includes(q)
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
      sku: '',
      name: '',
      category: '',
      description: '',
      barcode: '',
      imageUrl: '',
      price: 0,
      stock: 0,
      isActive: true
    });
    this.modalOpen = true;
  }

  openEdit(item: ProductListItem) {
    this.editing = item;
    this.form.reset({
      sku: item.sku || '',
      name: item.name || '',
      category: item.category || '',
      description: item.description || '',
      barcode: item.barcode || '',
      imageUrl: item.imageUrl || '',
      price: item.price ?? 0,
      stock: item.stock ?? 0,
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

    const payload: ProductUpsert = {
      sku: this.form.value.sku || '',
      name: this.form.value.name || '',
      category: this.form.value.category || '',
      description: this.form.value.description || '',
      barcode: this.form.value.barcode || '',
      imageUrl: this.form.value.imageUrl || '',
      price: Number(this.form.value.price ?? 0),
      stock: Number(this.form.value.stock ?? 0),
      isActive: !!this.form.value.isActive
    };

    try {
      if (this.editing) {
        await this.api.update(this.editing.id, payload);
        this.toasts.success('Producto actualizado');
      } else {
        await this.api.create(payload);
        this.toasts.success('Producto creado');
      }
      this.modalOpen = false;
      await this.reload();
    } finally {
      this.saving = false;
    }
  }

  async deactivate(item: ProductListItem) {
    const ok = confirm(`Desactivar ${item.name}?`);
    if (!ok) return;
    await this.api.remove(item.id);
    this.toasts.info('Producto desactivado');
    await this.reload();
  }
async toggleActive(item: ProductListItem) {
  const willActivate = !item.isActive;
  const action = willActivate ? 'Activar' : 'Desactivar';

  const ok = confirm(`${action} el producto ${item.name}?`);
  if (!ok) return;

  if (willActivate) {
    await this.api.activate(item.id);
    this.toasts.success('Producto activado');
  } else {
    await this.api.remove(item.id);
    this.toasts.info('Producto desactivado');
  }

  await this.reload();
}

}
