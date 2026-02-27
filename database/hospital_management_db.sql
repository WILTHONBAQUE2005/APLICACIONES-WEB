DROP DATABASE IF EXISTS hospital_management_db;
CREATE DATABASE hospital_management_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE hospital_management_db;

CREATE TABLE Pacientes (
    paciente_id INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    fecha_nacimiento DATE NOT NULL,
    telefono VARCHAR(20) NOT NULL,
    PRIMARY KEY (paciente_id)
) ENGINE=InnoDB;

CREATE TABLE Doctores (
    doctor_id INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    especialidad VARCHAR(120) NOT NULL,
    telefono VARCHAR(20) NOT NULL,
    PRIMARY KEY (doctor_id)
) ENGINE=InnoDB;

CREATE TABLE CitasMedicas (
    cita_id INT NOT NULL AUTO_INCREMENT,
    paciente_id INT NOT NULL,
    doctor_id INT NOT NULL,
    fecha_hora DATETIME NOT NULL,
    motivo VARCHAR(250) NOT NULL,
    estado VARCHAR(20) NOT NULL DEFAULT 'Programada',
    PRIMARY KEY (cita_id),
    CONSTRAINT FK_CitasMedicas_Pacientes FOREIGN KEY (paciente_id)
        REFERENCES Pacientes (paciente_id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,
    CONSTRAINT FK_CitasMedicas_Doctores FOREIGN KEY (doctor_id)
        REFERENCES Doctores (doctor_id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,
    CONSTRAINT CHK_CitasMedicas_Estado CHECK (estado IN ('Programada', 'Completada', 'Cancelada'))
) ENGINE=InnoDB;

CREATE INDEX IX_CitasMedicas_Paciente ON CitasMedicas (paciente_id);
CREATE INDEX IX_CitasMedicas_Doctor ON CitasMedicas (doctor_id);
CREATE INDEX IX_CitasMedicas_Doctor_FechaHora ON CitasMedicas (doctor_id, fecha_hora);
CREATE INDEX IX_CitasMedicas_Paciente_FechaHora ON CitasMedicas (paciente_id, fecha_hora);

INSERT INTO Pacientes (nombre, apellido, fecha_nacimiento, telefono) VALUES
('Ana', 'Moreira', '1998-06-14', '0991111111'),
('Carlos', 'Zambrano', '1989-11-03', '0982222222');

INSERT INTO Doctores (nombre, apellido, especialidad, telefono) VALUES
('Luis', 'Mendoza', 'Medicina General', '0973333333'),
('Sofia', 'Vera', 'Pediatria', '0964444444');

INSERT INTO CitasMedicas (paciente_id, doctor_id, fecha_hora, motivo, estado) VALUES
(1, 1, '2026-03-01 09:00:00', 'Chequeo general', 'Programada'),
(2, 2, '2026-03-01 10:30:00', 'Control pediatrico', 'Programada');
