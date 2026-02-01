# Sistema pedidos

## Qué incluye
- Dashboard (resumen general)
- CRUD de Clientes
- CRUD de Categorías
- CRUD de Productos
- Gestión de Pedidos (cabecera y detalles)
- Control básico de stock (según compras/ventas del pedido)

## Requisitos
- .NET SDK 8 (o el que use tu proyecto)
- XAMPP (MariaDB/MySQL)
- phpMyAdmin (incluido en XAMPP)

## Base de datos (MariaDB - XAMPP)
1. Abrir XAMPP y encender MySQL.
2. Crear la base de datos: `sistema_pedidos`.
3. Importar el script SQL (si aplica): `Database/database.sql`.

## Configurar conexión
Editar `appsettings.json` y verificar la cadena de conexión:
- Server: localhost
- Port: 3306
- Database: sistema_pedidos
- User: root
- Password: (vacía si tu XAMPP no tiene)

## Ejecutar por CMD
En la carpeta del proyecto:

```bash
dotnet restore
dotnet run
