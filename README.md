# Semana 5 y 6 – Página Web
Sistema web con login (cookies) y CRUD de **Clientes** y **Productos**.

---

## Requisitos
- Tener instalado **Node.js**
- Tener instalado **.NET SDK 8**
- Tener **XAMPP** (para MySQL/MariaDB)

---

## 1) Iniciar la Base de Datos (XAMPP)
1. Abre **XAMPP Control Panel**
2. Enciende **MySQL**
3. (Opcional) Abre **phpMyAdmin** y confirma que existe la base `pagina_web`
   - Si no existe, el backend la creará con migraciones si está configurado para eso.

---

## 2) Ejecutar el Backend (.NET)
1. Abre CMD
2. Ve a la carpeta del backend:

```bat
cd /d "C:\Users\baque\OneDrive\Escritorio\APLICACIONES WEB\Semana 5\pagina web\backend"


Ejecuta la API:

dotnet run --project "src\PaginaWeb.Api"


La API queda activa en un puerto local (sale en la consola).

Si tienes Swagger habilitado, normalmente es /swagger.

3) Ejecutar el Frontend (Angular)

Abre otra ventana CMD

Ve a la carpeta del frontend:

cd /d "C:\Users\baque\OneDrive\Escritorio\APLICACIONES WEB\Semana 5\pagina web\frontend"


Instala dependencias (solo la primera vez):

npm install


Inicia Angular:

npm start


Abre en el navegador:

http://localhost:4200

Login (demo)

Email: admin@demo.com

Password: Admin123!

Notas rápidas (si algo falla)

Si no cargan clientes o productos:

Abre DevTools (F12) → Network

Revisa la petición a /api/...

Si sale 500, normalmente es un dato NULL en BD o un error del backend.

Si sale 401/403, revisa que el backend esté corriendo y que el login esté hecho