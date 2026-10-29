# TicketHub

**Integrantes:** Benjamín Martínez y Santiago Pettinati  
**Materia:** Programación I - UCSE

## Estructura del proyecto

- `ApiGestion`: gestión de eventos y compras.
- `ApiValidacion`: validación de entradas.
- `Frontend`: página web para consultar eventos y comprar entradas.
- `Datos`: contiene los archivos JSON utilizados para guardar y consultar la información.

## Requisitos

Tener instalado el SDK de .NET 8, necesario para compilar y ejecutar ambas APIs.

## Primera ejecución

El archivo `Datos/datos.json` no se incluye en el repositorio. Al clonar el proyecto, el catálogo comienza vacío.

Para cargar un evento, abrir Swagger de Gestión y ejecutar `POST /api/eventos`, utilizando el DNI del organizador `30111222` en la query.

Ejemplo de body para crear un evento:

```json
{
  "nombre": "Evento de ejemplo",
  "descripcion": "Evento para probar la aplicación",
  "fecha": "2026-10-28T23:59:59",
  "lugar": "Club Central",
  "modalidades": [
    {
      "nombre": "General",
      "beneficios": "Acceso al evento",
      "precio": 1000,
      "cupoTotal": 10
    }
  ]
}
```

**Importante:** reemplazar la fecha del ejemplo por la fecha del día en que se realice la prueba y una hora futura. De esta manera, se podrán probar la compra de entradas y su validación durante la misma demostración.

## Ejecución de las APIs

Desde la carpeta raíz del proyecto, ejecutar cada comando en una terminal independiente:

**API de Gestión**

```bash
dotnet run --project ApiGestion/Gestion.Api --launch-profile http
```

Swagger de Gestión: [http://localhost:5175/swagger](http://localhost:5175/swagger)

**API de Validación**

```bash
dotnet run --project ApiValidacion/Validacion.Api --launch-profile http
```

Swagger de Validación: [http://localhost:5012/swagger](http://localhost:5012/swagger)

Para probar las funcionalidades de compra y validación, ambas APIs deben permanecer ejecutándose.

## Funcionamiento de la aplicación

### Gestión de eventos

Para crear, editar, cancelar eventos, agregar modalidades y consultar la recaudación, se debe pasar el DNI de un organizador en la query. Por ejemplo:

`dni=30111222`

### Compra de entradas

Para comprar entradas, enviar el DNI del comprador en el cuerpo de la solicitud.

Ejemplo de body:

```json
{
  "dni": 40123456,
  "eventoId": 1,
  "modalidadId": 1,
  "cantidad": 5
}
```

Las compras de cinco o más entradas de una misma modalidad tienen un descuento del 10 %. Cada entrada posee su propio código de seis caracteres.

### Validación de entradas

Para validar una entrada, se debe enviar el código de la entrada y el identificador del evento.

Ejemplo de body:

```json
{
  "codigo": "A7B2C9",
  "eventoId": 1
}
```

La validación se permite únicamente el día del evento y registra el ingreso. Un segundo intento de validar la misma entrada es rechazado.

El detalle de la compra muestra el estado actualizado de cada entrada.

Las fechas de los eventos se interpretan según la hora local de la computadora. Si se reciben en UTC, se convierten a la hora local para comprobar la disponibilidad y determinar si corresponde permitir la validación.

## Frontend

Abrir `Frontend/index.html` mediante la extensión **Live Server** de VS Code. Por ejemplo, se puede utilizar el puerto `5500`.

Para consultar eventos y comprar entradas, debe estar ejecutándose la API de Gestión en el puerto `5175`.

Para utilizar también la sección «Validar entrada», debe estar ejecutándose la API de Validación en el puerto `5012`.

## Ejecución de los tests

**Gestión**

```bash
dotnet test ApiGestion/Gestion.sln
```

**Validación**

```bash
dotnet test ApiValidacion/Validacion.sln
```

## Video de demostración

Video de demostración de TicketHub: https://programacion1-2026.slack.com/files/U0AKX6WHE76/F0CAU5GPAFJ/tickethub_-_google_chrome_2026-10-16_23-24-48.mp4


