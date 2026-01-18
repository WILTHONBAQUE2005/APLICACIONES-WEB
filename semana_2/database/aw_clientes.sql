CREATE DATABASE IF NOT EXISTS aw_clientes
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE aw_clientes;

CREATE TABLE IF NOT EXISTS clientes (
  id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  cedula VARCHAR(20) NOT NULL,
  nombres VARCHAR(80) NOT NULL,
  apellidos VARCHAR(80) NOT NULL,
  email VARCHAR(120) NOT NULL,
  pais_iso CHAR(2) NOT NULL,
  telefono VARCHAR(25) NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,

  UNIQUE KEY uq_clientes_cedula (cedula),
  UNIQUE KEY uq_clientes_email (email),
  UNIQUE KEY uq_clientes_telefono (telefono)
) ENGINE=InnoDB;
