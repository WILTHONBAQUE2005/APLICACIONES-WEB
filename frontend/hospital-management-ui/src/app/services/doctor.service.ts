import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api-base.service';
import { Doctor, DoctorPayload } from '../models/doctor.model';

@Injectable({ providedIn: 'root' })
export class DoctorService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${API_BASE_URL}/doctores`;

  obtenerTodos(): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(this.endpoint);
  }

  crear(payload: DoctorPayload): Observable<Doctor> {
    return this.http.post<Doctor>(this.endpoint, payload);
  }

  actualizar(id: number, payload: DoctorPayload): Observable<void> {
    return this.http.put<void>(`${this.endpoint}/${id}`, payload);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}
