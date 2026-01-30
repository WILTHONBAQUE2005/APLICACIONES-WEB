CREATE DATABASE IF NOT EXISTS ventas_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE ventas_db;

DROP TABLE IF EXISTS VentaDetalles;
DROP TABLE IF EXISTS Ventas;
DROP TABLE IF EXISTS Productos;
DROP TABLE IF EXISTS Clientes;

CREATE TABLE Clientes (
  cliente_id INT AUTO_INCREMENT PRIMARY KEY,
  cedula VARCHAR(10) NOT NULL,
  nombre VARCHAR(80) NOT NULL,
  apellido VARCHAR(80) NOT NULL,
  email VARCHAR(150) NOT NULL,
  telefono VARCHAR(30) NULL,
  direccion VARCHAR(200) NULL,
  activo TINYINT(1) NOT NULL DEFAULT 1,
  creado_en DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  UNIQUE KEY uq_clientes_cedula (cedula),
  UNIQUE KEY uq_clientes_email (email)
) ENGINE=InnoDB;

CREATE TABLE Productos (
  producto_id INT AUTO_INCREMENT PRIMARY KEY,
  codigo VARCHAR(32) NOT NULL,
  nombre VARCHAR(120) NOT NULL,
  descripcion VARCHAR(500) NULL,
  precio DECIMAL(18,2) NOT NULL,
  stock INT NOT NULL DEFAULT 0,
  activo TINYINT(1) NOT NULL DEFAULT 1,
  creado_en DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  UNIQUE KEY uq_productos_codigo (codigo)
) ENGINE=InnoDB;

CREATE TABLE Ventas (
  venta_id INT AUTO_INCREMENT PRIMARY KEY,
  numero_factura VARCHAR(30) NOT NULL,
  cliente_id INT NOT NULL,
  fecha DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  subtotal DECIMAL(18,2) NOT NULL,
  iva_tasa DECIMAL(5,4) NOT NULL,
  iva_valor DECIMAL(18,2) NOT NULL,
  total DECIMAL(18,2) NOT NULL,
  UNIQUE KEY uq_ventas_numero_factura (numero_factura),
  KEY ix_ventas_cliente (cliente_id),
  CONSTRAINT fk_ventas_cliente FOREIGN KEY (cliente_id) REFERENCES Clientes(cliente_id)
    ON UPDATE RESTRICT ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE VentaDetalles (
  venta_detalle_id INT AUTO_INCREMENT PRIMARY KEY,
  venta_id INT NOT NULL,
  producto_id INT NOT NULL,
  cantidad INT NOT NULL,
  precio_unitario DECIMAL(18,2) NOT NULL,
  subtotal DECIMAL(18,2) NOT NULL,
  UNIQUE KEY uq_venta_detalle_unico (venta_id, producto_id),
  KEY ix_detalle_venta (venta_id),
  KEY ix_detalle_producto (producto_id),
  CONSTRAINT fk_detalle_venta FOREIGN KEY (venta_id) REFERENCES Ventas(venta_id)
    ON UPDATE RESTRICT ON DELETE CASCADE,
  CONSTRAINT fk_detalle_producto FOREIGN KEY (producto_id) REFERENCES Productos(producto_id)
    ON UPDATE RESTRICT ON DELETE RESTRICT
) ENGINE=InnoDB;

INSERT INTO Clientes (cedula, nombre, apellido, email, telefono, direccion, activo) VALUES
('0901864603','Carlos','Mendoza','carlos.mendoza@mail.com','0987654321','Av. Principal 123',1),
('1710034065','Andrea','Vega','andrea.vega@mail.com','0991122334','Sector Centro',1);

INSERT INTO Productos (codigo, nombre, descripcion, precio, stock, activo) VALUES
('PROD-001','Mouse Inalámbrico','Mouse 2.4GHz',12.50,50,1),
('PROD-002','Teclado Mecánico','Switch azul, retroiluminado',35.99,20,1),
('PROD-003','Audífonos','Over-ear',24.00,30,1);
