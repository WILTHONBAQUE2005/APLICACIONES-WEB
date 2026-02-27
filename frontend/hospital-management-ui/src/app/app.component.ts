import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CitaMedica, CitaMedicaPayload } from './models/cita-medica.model';
import { Doctor, DoctorPayload } from './models/doctor.model';
import { Paciente, PacientePayload } from './models/paciente.model';
import { CitaMedicaService } from './services/cita-medica.service';
import { DoctorService } from './services/doctor.service';
import { PacienteService } from './services/paciente.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly pacienteService = inject(PacienteService);
  private readonly doctorService = inject(DoctorService);
  private readonly citaMedicaService = inject(CitaMedicaService);

  pacientes: Paciente[] = [];
  doctores: Doctor[] = [];
  citasMedicas: CitaMedica[] = [];

  pacienteEditandoId: number | null = null;
  doctorEditandoId: number | null = null;
  citaEditandoId: number | null = null;

  mensajeExito = '';
  mensajeError = '';
  cargando = false;

  readonly pacienteForm = this.fb.nonNullable.group({
    nombre: ['', [Validators.required, Validators.maxLength(100)]],
    apellido: ['', [Validators.required, Validators.maxLength(100)]],
    fechaNacimiento: ['', [Validators.required]],
    telefono: ['', [Validators.required, Validators.maxLength(20)]]
  });

  readonly doctorForm = this.fb.nonNullable.group({
    nombre: ['', [Validators.required, Validators.maxLength(100)]],
    apellido: ['', [Validators.required, Validators.maxLength(100)]],
    especialidad: ['', [Validators.required, Validators.maxLength(120)]],
    telefono: ['', [Validators.required, Validators.maxLength(20)]]
  });

  readonly citaForm = this.fb.nonNullable.group({
    pacienteId: [0, [Validators.required, Validators.min(1)]],
    doctorId: [0, [Validators.required, Validators.min(1)]],
    fechaHora: ['', [Validators.required]],
    motivo: ['', [Validators.required, Validators.maxLength(250)]],
    estado: ['Programada', [Validators.required]]
  });

  ngOnInit(): void {
    this.recargarVista();
  }

  get totalPacientes(): number {
    return this.pacientes.length;
  }

  get totalDoctores(): number {
    return this.doctores.length;
  }

  get totalCitas(): number {
    return this.citasMedicas.length;
  }

  onGuardarPaciente(): void {
    if (this.pacienteForm.invalid) {
      this.pacienteForm.markAllAsTouched();
      return;
    }

    const payload: PacientePayload = { ...this.pacienteForm.getRawValue() };
    this.limpiarMensajes();

    if (this.pacienteEditandoId) {
      this.pacienteService.actualizar(this.pacienteEditandoId, payload).subscribe({
        next: () => {
          this.pacienteForm.reset({ nombre: '', apellido: '', fechaNacimiento: '', telefono: '' });
          this.pacienteEditandoId = null;
          this.mensajeExito = 'Paciente actualizado correctamente.';
          this.recargarVista();
        },
        error: (error) => this.procesarError(error)
      });
      return;
    }

    this.pacienteService.crear(payload).subscribe({
      next: () => {
        this.pacienteForm.reset({ nombre: '', apellido: '', fechaNacimiento: '', telefono: '' });
        this.mensajeExito = 'Paciente registrado correctamente.';
        this.recargarVista();
      },
      error: (error) => this.procesarError(error)
    });
  }

  onEditarPaciente(paciente: Paciente): void {
    this.limpiarMensajes();
    this.pacienteEditandoId = paciente.pacienteId;
    this.pacienteForm.setValue({
      nombre: paciente.nombre,
      apellido: paciente.apellido,
      fechaNacimiento: this.obtenerSoloFecha(paciente.fechaNacimiento),
      telefono: paciente.telefono
    });
  }

  onCancelarEdicionPaciente(): void {
    this.pacienteEditandoId = null;
    this.pacienteForm.reset({ nombre: '', apellido: '', fechaNacimiento: '', telefono: '' });
  }

  onEliminarPaciente(id: number): void {
    if (!confirm('¿Deseas eliminar este paciente?')) {
      return;
    }

    this.limpiarMensajes();
    this.pacienteService.eliminar(id).subscribe({
      next: () => {
        this.mensajeExito = 'Paciente eliminado correctamente.';
        this.recargarVista();
      },
      error: (error) => this.procesarError(error)
    });
  }

  onGuardarDoctor(): void {
    if (this.doctorForm.invalid) {
      this.doctorForm.markAllAsTouched();
      return;
    }

    const payload: DoctorPayload = { ...this.doctorForm.getRawValue() };
    this.limpiarMensajes();

    if (this.doctorEditandoId) {
      this.doctorService.actualizar(this.doctorEditandoId, payload).subscribe({
        next: () => {
          this.doctorForm.reset({ nombre: '', apellido: '', especialidad: '', telefono: '' });
          this.doctorEditandoId = null;
          this.mensajeExito = 'Doctor actualizado correctamente.';
          this.recargarVista();
        },
        error: (error) => this.procesarError(error)
      });
      return;
    }

    this.doctorService.crear(payload).subscribe({
      next: () => {
        this.doctorForm.reset({ nombre: '', apellido: '', especialidad: '', telefono: '' });
        this.mensajeExito = 'Doctor registrado correctamente.';
        this.recargarVista();
      },
      error: (error) => this.procesarError(error)
    });
  }

  onEditarDoctor(doctor: Doctor): void {
    this.limpiarMensajes();
    this.doctorEditandoId = doctor.doctorId;
    this.doctorForm.setValue({
      nombre: doctor.nombre,
      apellido: doctor.apellido,
      especialidad: doctor.especialidad,
      telefono: doctor.telefono
    });
  }

  onCancelarEdicionDoctor(): void {
    this.doctorEditandoId = null;
    this.doctorForm.reset({ nombre: '', apellido: '', especialidad: '', telefono: '' });
  }

  onEliminarDoctor(id: number): void {
    if (!confirm('¿Deseas eliminar este doctor?')) {
      return;
    }

    this.limpiarMensajes();
    this.doctorService.eliminar(id).subscribe({
      next: () => {
        this.mensajeExito = 'Doctor eliminado correctamente.';
        this.recargarVista();
      },
      error: (error) => this.procesarError(error)
    });
  }

  onGuardarCita(): void {
    if (this.citaForm.invalid) {
      this.citaForm.markAllAsTouched();
      return;
    }

    const payload: CitaMedicaPayload = {
      pacienteId: Number(this.citaForm.controls.pacienteId.getRawValue()),
      doctorId: Number(this.citaForm.controls.doctorId.getRawValue()),
      fechaHora: this.citaForm.controls.fechaHora.getRawValue(),
      motivo: this.citaForm.controls.motivo.getRawValue(),
      estado: this.citaForm.controls.estado.getRawValue()
    };

    this.limpiarMensajes();

    if (this.citaEditandoId) {
      this.citaMedicaService.actualizar(this.citaEditandoId, payload).subscribe({
        next: () => {
          this.reiniciarFormularioCita();
          this.mensajeExito = 'Cita medica actualizada correctamente.';
          this.recargarVista();
        },
        error: (error) => this.procesarError(error)
      });
      return;
    }

    this.citaMedicaService.crear(payload).subscribe({
      next: () => {
        this.reiniciarFormularioCita();
        this.mensajeExito = 'Cita medica registrada correctamente.';
        this.recargarVista();
      },
      error: (error) => this.procesarError(error)
    });
  }

  onEditarCita(cita: CitaMedica): void {
    this.limpiarMensajes();
    this.citaEditandoId = cita.citaId;
    this.citaForm.setValue({
      pacienteId: cita.pacienteId,
      doctorId: cita.doctorId,
      fechaHora: this.obtenerFechaHoraLocal(cita.fechaHora),
      motivo: cita.motivo,
      estado: cita.estado
    });
  }

  onCancelarEdicionCita(): void {
    this.reiniciarFormularioCita();
  }

  onEliminarCita(id: number): void {
    if (!confirm('¿Deseas eliminar esta cita medica?')) {
      return;
    }

    this.limpiarMensajes();
    this.citaMedicaService.eliminar(id).subscribe({
      next: () => {
        this.mensajeExito = 'Cita medica eliminada correctamente.';
        this.recargarVista();
      },
      error: (error) => this.procesarError(error)
    });
  }

  private recargarVista(): void {
    this.cargando = true;
    this.cargarPacientes();
    this.cargarDoctores();
    this.cargarCitasMedicas();
  }

  private cargarPacientes(): void {
    this.pacienteService.obtenerTodos().subscribe({
      next: (data) => {
        this.pacientes = data;
        this.finalizarCarga();
      },
      error: (error) => this.procesarError(error)
    });
  }

  private cargarDoctores(): void {
    this.doctorService.obtenerTodos().subscribe({
      next: (data) => {
        this.doctores = data;
        this.finalizarCarga();
      },
      error: (error) => this.procesarError(error)
    });
  }

  private cargarCitasMedicas(): void {
    this.citaMedicaService.obtenerTodas().subscribe({
      next: (data) => {
        this.citasMedicas = data;
        this.finalizarCarga();
      },
      error: (error) => this.procesarError(error)
    });
  }

  private finalizarCarga(): void {
    this.cargando = false;
  }

  private reiniciarFormularioCita(): void {
    this.citaEditandoId = null;
    this.citaForm.reset({
      pacienteId: 0,
      doctorId: 0,
      fechaHora: '',
      motivo: '',
      estado: 'Programada'
    });
  }

  private obtenerSoloFecha(valor: string): string {
    return valor ? valor.substring(0, 10) : '';
  }

  private obtenerFechaHoraLocal(valor: string): string {
    return valor ? valor.substring(0, 16) : '';
  }

  private limpiarMensajes(): void {
    this.mensajeExito = '';
    this.mensajeError = '';
  }

  private procesarError(error: unknown): void {
    this.cargando = false;

    if (error instanceof HttpErrorResponse) {
      const mensaje = error.error?.mensaje;
      this.mensajeError = typeof mensaje === 'string'
        ? mensaje
        : 'Ocurrio un error al procesar la solicitud.';
      return;
    }

    this.mensajeError = 'Ocurrio un error inesperado.';
  }
}
