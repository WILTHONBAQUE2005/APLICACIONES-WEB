import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="min-h-screen bg-slate-950 text-slate-100 flex items-center justify-center p-6">
      <div class="max-w-md w-full rounded-2xl border border-slate-800 bg-slate-900/40 p-8 text-center shadow-xl">
        <div class="text-6xl font-extrabold tracking-tight">404</div>
        <div class="mt-2 text-xl font-semibold">Página no encontrada</div>
        <p class="mt-3 text-slate-300">
          La ruta que intentas abrir no existe o fue movida.
        </p>
        <a routerLink="/login"
           class="inline-flex items-center justify-center mt-6 px-5 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 transition">
          Ir al login
        </a>
      </div>
    </div>
  `
})
export class NotFoundComponent {}
