export interface Doctor {
  doctorId: number;
  nombre: string;
  apellido: string;
  especialidad: string;
  telefono: string;
}

export interface DoctorPayload {
  nombre: string;
  apellido: string;
  especialidad: string;
  telefono: string;
}
