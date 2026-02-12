USE pagina_web;

-- =========================
-- CLIENTS (8 registros)
-- =========================
INSERT INTO clients (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, DocumentType, DocumentNumber, FirstName, LastName, Email, Phone, AddressLine1, City, Notes)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'CI', '0912345678', 'Ana', 'Vera', 'ana.vera@demo.com', '0991112233', 'Av. Amazonas N34-120', 'Quito', 'Cliente demo'
WHERE NOT EXISTS (SELECT 1 FROM clients WHERE DocumentNumber='0912345678');

INSERT INTO clients (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, DocumentType, DocumentNumber, FirstName, LastName, Email, Phone, AddressLine1, City, Notes)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'CI', '0923456789', 'Carlos', 'Mena', 'carlos.mena@demo.com', '0982223344', 'La Pradera Mz 12', 'Guayaquil', 'Cliente frecuente'
WHERE NOT EXISTS (SELECT 1 FROM clients WHERE DocumentNumber='0923456789');

INSERT INTO clients (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, DocumentType, DocumentNumber, FirstName, LastName, Email, Phone, AddressLine1, City, Notes)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'CI', '0934567890', 'Diana', 'Paz', 'diana.paz@demo.com', '0973334455', 'Cdla. Los Ceibos', 'Guayaquil', ''
WHERE NOT EXISTS (SELECT 1 FROM clients WHERE DocumentNumber='0934567890');

INSERT INTO clients (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, DocumentType, DocumentNumber, FirstName, LastName, Email, Phone, AddressLine1, City, Notes)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'CI', '0945678901', 'Erick', 'Santos', 'erick.santos@demo.com', '0964445566', 'Barrio Centro', 'Cuenca', ''
WHERE NOT EXISTS (SELECT 1 FROM clients WHERE DocumentNumber='0945678901');

INSERT INTO clients (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, DocumentType, DocumentNumber, FirstName, LastName, Email, Phone, AddressLine1, City, Notes)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'CI', '0956789012', 'Fernanda', 'Loor', 'fernanda.loor@demo.com', '0955556677', 'Av. 6 de Diciembre', 'Quito', ''
WHERE NOT EXISTS (SELECT 1 FROM clients WHERE DocumentNumber='0956789012');

INSERT INTO clients (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, DocumentType, DocumentNumber, FirstName, LastName, Email, Phone, AddressLine1, City, Notes)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'CI', '0967890123', 'Gustavo', 'Alcívar', 'gustavo.alcivar@demo.com', '0946667788', 'Malecón y Junín', 'Machala', ''
WHERE NOT EXISTS (SELECT 1 FROM clients WHERE DocumentNumber='0967890123');

INSERT INTO clients (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, DocumentType, DocumentNumber, FirstName, LastName, Email, Phone, AddressLine1, City, Notes)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'CI', '0978901234', 'Helena', 'Rivas', 'helena.rivas@demo.com', '0937778899', 'Sector Norte', 'Loja', ''
WHERE NOT EXISTS (SELECT 1 FROM clients WHERE DocumentNumber='0978901234');

INSERT INTO clients (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, DocumentType, DocumentNumber, FirstName, LastName, Email, Phone, AddressLine1, City, Notes)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'CI', '0989012345', 'Iván', 'Cedeño', 'ivan.cedeno@demo.com', '0928889900', 'Av. Principal S/N', 'Manta', ''
WHERE NOT EXISTS (SELECT 1 FROM clients WHERE DocumentNumber='0989012345');

-- =========================
-- PRODUCTS (10 registros)
-- =========================
INSERT INTO products (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, Sku, Name, Category, Description, Barcode, ImageUrl, Price, Stock)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'PRD-0001', 'Laptop 15.6"', 'Electrónica', 'Laptop para oficina', '7501234567890', NULL, 650.00, 5
WHERE NOT EXISTS (SELECT 1 FROM products WHERE Sku='PRD-0001');

INSERT INTO products (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, Sku, Name, Category, Description, Barcode, ImageUrl, Price, Stock)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'PRD-0002', 'Mouse inalámbrico', 'Accesorios', 'Mouse 2.4GHz', '7501234567891', NULL, 12.50, 40
WHERE NOT EXISTS (SELECT 1 FROM products WHERE Sku='PRD-0002');

INSERT INTO products (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, Sku, Name, Category, Description, Barcode, ImageUrl, Price, Stock)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'PRD-0003', 'Teclado mecánico', 'Accesorios', 'Switch azul', '7501234567892', NULL, 45.99, 20
WHERE NOT EXISTS (SELECT 1 FROM products WHERE Sku='PRD-0003');

INSERT INTO products (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, Sku, Name, Category, Description, Barcode, ImageUrl, Price, Stock)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'PRD-0004', 'Monitor 24"', 'Electrónica', 'Full HD', '7501234567893', NULL, 129.90, 12
WHERE NOT EXISTS (SELECT 1 FROM products WHERE Sku='PRD-0004');

INSERT INTO products (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, Sku, Name, Category, Description, Barcode, ImageUrl, Price, Stock)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'PRD-0005', 'Disco SSD 512GB', 'Almacenamiento', 'SATA', '7501234567894', NULL, 54.00, 25
WHERE NOT EXISTS (SELECT 1 FROM products WHERE Sku='PRD-0005');

INSERT INTO products (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, Sku, Name, Category, Description, Barcode, ImageUrl, Price, Stock)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'PRD-0006', 'Audífonos', 'Audio', 'Over-ear', '7501234567895', NULL, 22.75, 30
WHERE NOT EXISTS (SELECT 1 FROM products WHERE Sku='PRD-0006');

INSERT INTO products (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, Sku, Name, Category, Description, Barcode, ImageUrl, Price, Stock)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'PRD-0007', 'USB 64GB', 'Almacenamiento', 'USB 3.0', '7501234567896', NULL, 9.99, 60
WHERE NOT EXISTS (SELECT 1 FROM products WHERE Sku='PRD-0007');

INSERT INTO products (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, Sku, Name, Category, Description, Barcode, ImageUrl, Price, Stock)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'PRD-0008', 'Impresora', 'Oficina', 'Multifunción', '7501234567897', NULL, 180.00, 6
WHERE NOT EXISTS (SELECT 1 FROM products WHERE Sku='PRD-0008');

INSERT INTO products (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, Sku, Name, Category, Description, Barcode, ImageUrl, Price, Stock)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'PRD-0009', 'Router WiFi', 'Redes', 'Dual band', '7501234567898', NULL, 35.50, 18
WHERE NOT EXISTS (SELECT 1 FROM products WHERE Sku='PRD-0009');

INSERT INTO products (Id, CreatedAtUtc, UpdatedAtUtc, IsActive, Sku, Name, Category, Description, Barcode, ImageUrl, Price, Stock)
SELECT UUID(), UTC_TIMESTAMP(6), UTC_TIMESTAMP(6), 1, 'PRD-0010', 'Cámara Web', 'Accesorios', '1080p', '7501234567899', NULL, 28.25, 15
WHERE NOT EXISTS (SELECT 1 FROM products WHERE Sku='PRD-0010');

-- Si tu MySQL/MariaDB no acepta UTC_TIMESTAMP(6), cambia por UTC_TIMESTAMP() en todo.
