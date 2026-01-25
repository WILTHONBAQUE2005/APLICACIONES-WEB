# Gestión de clientes (Web)

Stack:
- Frontend: JavaScript (fetch) + Bootstrap + DataTables
- Backend: ASP.NET Core (API)
- BD: MariaDB (XAMPP)

Incluye:
- CRUD completo (crear, editar, guardar, listar)
- “Eliminar” mejorado como **desactivar/activar** (soft delete)
- Catálogos:
  - Ciudad (list)
  - Cantón (list dependiente por ciudad)
  - Tipo de cliente (list)

## Requisitos
- .NET SDK 8 (recomendado)
- XAMPP (MySQL/MariaDB)

## Paso a paso (CMD)

1) Inicia **MySQL** en XAMPP.

2) Crea la base de datos:
```bat
"C:\xampp\mysql\bin\mysql.exe" -u root
```

Dentro de MariaDB:
```sql
CREATE DATABASE IF NOT EXISTS crud_clientes_db
CHARACTER SET utf8mb4
COLLATE utf8mb4_general_ci;
EXIT;
```

3) Entra a la carpeta del proyecto:
```bat
cd /d "C:\RUTA\A\ClientesCrudWeb"
```

4) Restaura paquetes:
```bat
dotnet restore
```

5) Migraciones / crear tablas:
```bat
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

6) Ejecuta:
```bat
dotnet run
```

Abre:
- http://localhost:5279

## Datos de prueba (100 clientes)

Después de `dotnet ef database update`, ejecuta el script:
- `sql/seed_100_clientes.sql`

Ejemplo (CMD):
```bat
"C:\xampp\mysql\bin\mysql.exe" -u root crud_clientes_db < sql\seed_100_clientes.sql
```
