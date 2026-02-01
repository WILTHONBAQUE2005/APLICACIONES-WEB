
# SistemaPedidosPro (ASP.NET Core MVC + MariaDB)

Un sistema de pedidos realista con:
- **Clientes**, **Productos**, **Categorías**
- **Pedidos** con detalle (items), cálculo de totales y control de stock
- **Pagos** (parcial / total) y cambio automático de estado
- **Envíos** (tracking / transportista / estado)
- **Inventario** (movimientos y alertas de bajo stock)
- **Dashboard** con KPIs y gráficos (Chart.js)

## Requisitos
- .NET SDK 8
- MariaDB o MySQL (recomendado: XAMPP con MariaDB)
- Visual Studio 2022 o VS Code

## Configuración rápida (XAMPP)
1. Inicia **Apache** y **MySQL** desde XAMPP.
2. Entra a phpMyAdmin y crea la base:
   - `sistema_pedidos`
3. Abre `appsettings.json` y ajusta la cadena de conexión si tu usuario/clave son diferentes:

```json
"MariaDb": "Server=localhost;Port=3306;Database=sistema_pedidos;User=root;Password=;TreatTinyAsBoolean=true;SslMode=none"
```

## Ejecutar
Desde la carpeta del proyecto:

```bash
dotnet restore
dotnet run
```

El sistema crea tablas automáticamente al iniciar (EnsureCreated) y **carga datos de ejemplo** (categorías, productos y clientes).

## Notas importantes
- Para evitar errores típicos con decimales (por cultura), el proyecto fija la cultura con separador decimal `.`.
- Puedes extender el sistema con:
  - Migrations (`dotnet ef migrations add InitialCreate` y `dotnet ef database update`)
  - Facturación, roles/usuarios, impresión PDF, etc.

## Estructura
- `Models/` Entidades del dominio
- `Data/` DbContext + Seed
- `Services/` lógica (pedidos, dashboard, inventario)
- `Controllers/` endpoints MVC
- `Views/` interfaz (Bootstrap 5)

Hecho para que lo puedas usar como base de un proyecto universitario o producción pequeña.
