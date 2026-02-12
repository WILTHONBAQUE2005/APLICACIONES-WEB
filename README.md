# Página Web (Angular + .NET + MariaDB)

Proyecto académico: **CRUD profesional** de **Clientes** y **Productos** con **autenticación por cookies**, rutas protegidas, interceptor, guard, validación reactiva, manejo de errores y base de datos MariaDB (XAMPP).

## Estructura

- `backend/` API .NET 8 (cookie auth + XSRF + EF Core + MariaDB)
- `frontend/` Angular (login + dashboard + CRUD clientes/productos)
- `database/` script opcional para crear la base

## Requisitos

- Windows 10/11
- **XAMPP** (MariaDB/MySQL) corriendo en `localhost:3306`
- **.NET SDK 8**
- **Node.js LTS** (18+)
- (Opcional) Git

## Paso a paso (para que corra a la primera)

### 1) Base de datos (XAMPP)

1. Abre XAMPP y enciende **MySQL**.
2. Entra a `http://localhost/phpmyadmin`.
3. Crea una base llamada: `pagina_web`.
4. Si tu MariaDB tiene contraseña en `root`, anótala.

También puedes ejecutar `database/init.sql` desde phpMyAdmin.

### 2) Backend (.NET)

En una terminal:

```bash
cd backend/src/PaginaWeb.Api
dotnet restore
dotnet run
```

- La API se levanta (por defecto) en `http://localhost:5200`.
- En el primer arranque aplica migraciones automáticamente y crea un usuario admin si no existe.

**Credenciales demo (puedes cambiarlas en la UI luego):**
- Email: `admin@demo.com`
- Password: `Admin123!`

Si tu usuario/clave de MariaDB es diferente, edita:
`backend/src/PaginaWeb.Api/appsettings.Development.json`

### 3) Frontend (Angular)

En otra terminal:

```bash
cd frontend
npm install
npm start
```

Esto levanta Angular en `http://localhost:4200` usando proxy hacia la API (`/api`), para que cookies funcionen sin CORS.

### 4) Probar

1. Abre `http://localhost:4200`
2. Login con `admin@demo.com` / `Admin123!`
3. Entra a **Clientes** y **Productos** y prueba crear/editar/eliminar.

## Postman y .http

- `backend/PaginaWeb.postman_collection.json`
- `backend/PaginaWeb.http`

## Subir a GitHub (link para entregar)

1. Crea un repo vacío en GitHub (ej. `pagina-web`).
2. En la carpeta extraída:

```bash
git init
git add .
git commit -m "Entrega CRUD Angular + .NET + MariaDB"
git branch -M main
git remote add origin https://github.com/TU_USUARIO/TU_REPO.git
git push -u origin main
```

Luego entrega el enlace.

