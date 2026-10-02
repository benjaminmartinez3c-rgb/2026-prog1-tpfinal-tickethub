# TicketHub

Trabajo final de Programación 1. Dos APIs en ASP.NET Core (.NET 8).

Desde la raíz del repositorio, abrir dos terminales:

```
dotnet run --project ApiGestion/Gestion.Api --launch-profile http
dotnet run --project ApiValidacion/Validacion.Api --launch-profile http
```

Swagger de gestión: http://localhost:5175/swagger
Swagger de validación: http://localhost:5012/swagger

Las dos APIs usan la carpeta `Datos`, relativa a los proyectos. `usuarios.json` contiene los usuarios de la consigna. `datos.json` se genera al guardar eventos o compras y conserva la información al reiniciar.

Para crear, editar, cancelar eventos, agregar modalidades y consultar recaudación, pasar el DNI de un organizador en la query (`dni=30111222`). Para comprar, usar el DNI de un comprador en el cuerpo:

```json
{ "dni": 40123456, "eventoId": 1, "modalidadId": 1, "cantidad": 5 }
```

Las compras de 5 o más entradas de la misma modalidad tienen 10% de descuento. Cada entrada tiene su propio código de seis caracteres. Para validar, pasar el evento y uno de esos códigos:

```json
{ "codigo": "000001", "eventoId": 1 }
```

La validación se permite el día del evento y registra el ingreso. Un segundo intento se rechaza. El detalle de la compra muestra el estado actualizado de cada entrada.

Tests:

```
dotnet test ApiGestion/Gestion.sln
dotnet test ApiValidacion/Validacion.sln
```

Entrega del 2/10: APIs. El frontend corresponde a la entrega del 16/10.

Si la PC solo tiene .NET 10, antes de los comandos anteriores ejecutar en cada terminal PowerShell:

```
$env:DOTNET_ROLL_FORWARD = "Major"
```

Con .NET 8 instalado no hace falta ese paso.

## Cómo está organizado

`Program.cs` inicia la aplicación, habilita Swagger y conecta los controladores.
Los controladores reciben los pedidos HTTP y devuelven las respuestas. La lógica está en los servicios y las clases de entidades guardan la información.

Por ejemplo, al comprar:

1. `GestionController.Comprar` recibe DNI, evento, modalidad y cantidad.
2. `ServicioUsuario` comprueba que el DNI sea de un comprador.
3. `ArchivoDatos.LeerDatos` carga el JSON.
4. `ServicioCompra.Comprar` verifica evento y cupo, calcula el precio y crea una entrada por persona.
5. `ArchivoDatos.GuardarDatos` guarda los cambios.
6. El controlador devuelve la compra.

Cada operación vuelve a leer el archivo para ver los cambios hechos por la otra API. El archivo `datos.lock` se mantiene abierto durante la operación: así las dos APIs no pueden guardar al mismo tiempo. `using` lo cierra al terminar, incluso si ocurre un error.

Los códigos usan los caracteres 0-9 y A-Z. Se guarda un contador que aumenta con cada entrada y se convierte a seis caracteres. El contador no se reinicia con las APIs, por eso no se reutilizan códigos. Si se agotan las combinaciones se rechaza la compra.

En los controladores, `Ok` responde 200, `Created` responde 201, `NoContent` responde 204, `BadRequest` responde 400, `NotFound` responde 404 y `StatusCode(403, ...)` indica falta de permiso. `try/catch` permite devolver el motivo cuando una regla rechaza el pedido.
