# Tarea 1 (Semana 1) - Web personal + Calculadora (PHP) + MariaDB

## Descripción
En esta tarea desarrollé una página web personal usando **ASP.NET Core MVC (.NET 8)** con **HTML y CSS**.  
Además, integré una **calculadora en PHP** con validaciones y guardado de historial en **MariaDB** (XAMPP).  
La calculadora se muestra dentro de la web mediante un **iframe**.

---

## Requisitos
- Visual Studio 2022 (o superior)
- .NET SDK 8.0
- XAMPP (Apache + MariaDB)
- Git (opcional, para clonar el repositorio)

---

## Base de datos (MariaDB - XAMPP)
1. Abrir **XAMPP** y encender **MySQL** (MariaDB).
2. Entrar a **phpMyAdmin**.
3. Crear la base de datos y tabla ejecutando este script:

```sql
CREATE DATABASE IF NOT EXISTS tarea1
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE tarea1;

CREATE TABLE IF NOT EXISTS calculations (
  id INT AUTO_INCREMENT PRIMARY KEY,
  operand1 DECIMAL(18,4) NOT NULL,
  operand2 DECIMAL(18,4) NOT NULL,
  operator VARCHAR(5) NOT NULL,
  result DECIMAL(18,4) NOT NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);


## Ejecutar PHP (calculadora)
Este es el comando para correr PHP:

```bat
"C:\xampp\php\php.exe" -S localhost:8001 -t ".\php"

Link:

http://localhost:8001/index.php
