PASO A PASO (Windows + XAMPP + MariaDB)

1) Requisitos
- .NET SDK 8 instalado (dotnet --version debe mostrar 8.x)
- XAMPP (inicia MySQL)

2) Base de datos (MariaDB en XAMPP)
- Abre http://localhost/phpmyadmin
- Ve a Importar
- Importa el archivo: database/schema.sql
Esto crea la BD ventas_db con tablas: Clientes, Productos, Ventas, VentaDetalles y datos de ejemplo.

3) Configurar conexión
- Abre: src/SalesPro.Web/appsettings.json
- Revisa ConnectionStrings:MariaDb (por defecto)
  server=localhost;port=3306;database=ventas_db;user=root;password=;...

Si tu MySQL tiene clave en root, ponla en password.

4) Ejecutar el proyecto
En CMD o PowerShell:
cd "RUTA\VentasPro\src\SalesPro.Web"
dotnet restore
dotnet run

5) Abrir la app
- En consola te saldrá el puerto.
- Por defecto está configurado para:
  http://localhost:5212
  https://localhost:7212

6) Uso realista (ventas)
- Para registrar una venta:
  1) Busca cliente por cédula o por nombres y selecciona.
  2) Busca productos por código o nombre y agrégalos.
  3) Ajusta cantidades (valida stock).
  4) IVA calculado y total automático.
  5) Registrar venta.

Reglas aplicadas
- No permite clientes duplicados por cédula ni email.
- No permite productos duplicados por código.
- No permite productos repetidos dentro de una misma venta.
- Control de stock en servidor (transacción + bloqueo FOR UPDATE).

