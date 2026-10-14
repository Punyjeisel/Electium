# Electium

Proyecto de votación electrónica hecho con ASP.NET Core MVC en .NET 9.

## Qué necesitas antes de empezar

- Visual Studio 2022
- .NET 9 SDK
- SQL Server (Express o Developer sirve)
- Git

## Cómo correrlo

1. Clona el repo:
   git clone https://github.com/tu-usuario/Electium.git

2. Abre `Electium.sln` en Visual Studio y deja que restaure los paquetes.

3. Revisa la cadena de conexión en `Electium.Presentation.WebApp/appsettings.json`. Por defecto está apuntando a SQL Server local. Si usas Express, cámbiala a algo como:
   Server=.\SQLEXPRESS;Database=ElectiumDb;Trusted_Connection=True;TrustServerCertificate=True;

4. Descarga el archivo `spa.traineddata` de aquí:
   https://github.com/tesseract-ocr/tessdata/blob/main/spa.traineddata
   Y ponlo en `Electium.Presentation.WebApp/wwwroot/tessdata/` (crea la carpeta si no existe). Esto es para que el OCR pueda leer las cédulas.

5. Abre la consola de paquetes NuGet en Visual Studio (Herramientas > Administrador de paquetes NuGet > Consola). Arriba selecciona:
   - Proyecto predeterminado: Electium.Infrastructure.Persistence
   - Proyecto de inicio: Electium.Presentation.WebApp
   Y corre:
   Update-Database

6. Dale F5. La app arranca y crea solo el usuario admin.

## Usuario por defecto

Usuario: admin
Contraseña: Admin123!

Esa es la única cuenta que se crea automáticamente. Todo lo demás lo tienes que crear desde la app.

## Sobre el correo (opcional)

Si quieres que funcione el envío del resumen por correo cuando alguien vota, necesitas configurar un correo de Gmail con contraseña de aplicación.

Clic derecho sobre el proyecto Web > Administrar secretos de usuario, y pega algo así:

{
  "EmailSettings": {
    "SenderEmail": "tucorreo@gmail.com",
    "Password": "xxxx xxxx xxxx xxxx"
  }
}

La contraseña esa no es la normal de tu Gmail, es una "App Password". Se genera en tu cuenta de Google, en Seguridad > Verificación en dos pasos > Contraseñas de aplicaciones.

Si no lo configuras no pasa nada, la app funciona igual, solo que no manda el correo al final de la votación.

## Cómo se usa

Con el admin entras y creas:
- Puestos electivos
- Partidos políticos (con logo)
- Usuarios dirigentes
- Ciudadanos

Después asignas cada dirigente a un partido, y ellos entran con su usuario a crear candidatos, asignarlos a puestos y gestionar alianzas.

Cuando ya haya al menos 2 partidos con candidatos para todos los puestos activos, el admin puede crear una elección.

El ciudadano entra a la página principal, pone su cédula, sube una foto de la cédula, el sistema la lee con OCR, y si todo coincide puede votar por cada puesto.

Al terminar, si configuraste el correo, le llega un resumen.

## Cosas a saber

- Un ciudadano solo puede votar una vez por elección.
- Mientras hay una elección activa no se puede crear ni editar nada (puestos, ciudadanos, partidos, etc).
- El voto es confidencial, no se guarda quién votó por quién.
- Nada se borra de verdad, todo se marca como inactivo.
- Si inactivas un partido, sus candidatos también se inactivan.

## Si algo falla

- Si Update-Database te da error, revisa que hayas seleccionado bien los proyectos en la consola.
- Si el OCR no lee, prueba con una foto más clara y bien iluminada.
- Si el correo no llega, revisa los User Secrets y que la contraseña sea de aplicación, no la normal.

## Autor

Jeisel Barley 2026