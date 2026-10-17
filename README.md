TicketHub

Integrantes: Benjamín Martínez y Santiago Pettinati
Materia: Programación I - UCSE

## Estructura del proyecto

- `ApiGestion`: gestión de eventos y compras.
- `ApiValidacion`: validación de entradas.
- `Frontend`: página web para consultar eventos y comprar entradas.
- `Datos`: contiene los archivos JSON. Ambas APIs utilizan el mismo `datos.json` para guardar y consultar la información.

Requisito: tener instalado el SDK de .NET 8, que incluye los runtimes necesarios para compilar y ejecutar ambas APIs.

```
dotnet run --project ApiGestion/Gestion.Api --launch-profile http
dotnet run --project ApiValidacion/Validacion.Api --launch-profile http
```

Swagger de gestión: http://localhost:5175/swagger
Swagger de validación: http://localhost:5012/swagger


Para crear, editar, cancelar eventos, agregar modalidades y consultar recaudación, pasar el DNI de un organizador en la query (`dni=30111222`). Para comprar, usar el DNI de un comprador en el cuerpo:

```json
{ "dni": 40123456, "eventoId": 1, "modalidadId": 1, "cantidad": 5 }
```

Las compras de 5 o más entradas de la misma modalidad tienen 10% de descuento. Cada entrada tiene su propio código de seis caracteres. Para validar, pasar el evento y uno de esos códigos:

```json
{ "codigo": "A7B2C9", "eventoId": 1 }
```

La validación se permite el día del evento y registra el ingreso. Un segundo intento se rechaza. El detalle de la compra muestra el estado actualizado de cada entrada.

Ejecutar los tests

Gestión:

dotnet test ApiGestion/Gestion.sln

Validación:

dotnet test ApiValidacion/Validacion.sln

Abrir el frontend

Abrir Frontend/index.html con la extensión Live Server de VS Code, con la API de Gestión ejecutándose.
Para usar también la sección «Validar entrada», debe estar ejecutándose la API de Validación en el puerto 5012, además de la API de Gestión en el puerto 5175.
Las fechas de los eventos se interpretan en la hora local de la computadora. Si se reciben en UTC, se convierten a hora local para comprobar la disponibilidad y el día de validación
