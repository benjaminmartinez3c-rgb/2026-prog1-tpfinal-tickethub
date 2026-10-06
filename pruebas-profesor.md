# Pruebas del profesor — 1ra entrega (Web APIs)

Grupo: **tickethub** (Benjamin Martinez, Santiago Pettinati)

## Entorno y forma de ejecución

- Estado del repositorio al momento de probar: rama `main`, último commit `cc9b109` (02/10/2026 00:09 -03). No hay commits posteriores a la fecha de entrega.
- SDK disponible en la máquina de corrección: .NET 10.0.303. Los proyectos apuntan a `net8.0` y en la máquina no está el runtime de ASP.NET Core 8, por lo que las APIs se ejecutaron con la variable `DOTNET_ROLL_FORWARD=Major`. Es una cuestión del entorno de corrección y no del código.
- Compilación: `dotnet build ApiGestion/Gestion.sln` y `dotnet build ApiValidacion/Validacion.sln` → **compilación correcta, 0 advertencias, 0 errores**.
- Tests: `dotnet test ApiGestion/Gestion.sln` → **14/14 en verde**. `dotnet test ApiValidacion/Validacion.sln` → **8/8 en verde**.
- Las dos APIs se levantaron directamente desde el repositorio, desde la raíz:
  - `dotnet run --project ApiGestion/Gestion.Api --no-launch-profile --urls http://localhost:5190`
  - `dotnet run --project ApiValidacion/Validacion.Api --no-launch-profile --urls http://localhost:5191`
- Las dos resuelven la carpeta `Datos` de la raíz del repositorio (ruta relativa al proyecto), así que trabajan sobre el mismo `datos.json`.
- Pruebas realizadas el 04/10/2026 entre las 08:33 y las 08:37. Para probar la validación en puerta se crearon eventos con fecha del mismo día, porque el grupo solo permite validar el día del evento. El evento 2 se creó con fecha 04/10 08:36, para poder validar sus entradas antes de esa hora y después comprobar que ya no se puede comprar cuando la fecha pasó.
- La API recibe el DNI de quien opera por **query string** (`?dni=...`) en los endpoints de organizador y en el **body** para la compra.
- Al terminar se borraron los archivos de datos generados por las pruebas (`Datos/datos.json`, `Datos/datos.lock`), que están en el `.gitignore` del grupo y no existían antes.

## Pruebas

### 1. Swagger gestion

```bash
curl -o /dev/null http://localhost:5190/swagger/index.html
```

Respuesta:

```
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 2. Swagger json gestion

```bash
curl http://localhost:5190/swagger/v1/swagger.json -o /dev/null
```

Respuesta:

```
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 3. GET usuarios

```bash
curl http://localhost:5190/api/usuarios
```

Respuesta:

```
[{"dni":30111222,"nombre":"Lucía Fernández","username":"lfernandez","rol":"Organizador"},{"dni":29888777,"nombre":"Martín Aguirre","username":"maguirre","rol":"Organizador"},{"dni":40123456,"nombre":"Sofía Gómez","username":"sgomez","rol":"Comprador"},{"dni":38456789,"nombre":"Nicolás Pereyra","username":"npereyra","rol":"Comprador"},{"dni":41234567,"nombre":"Valentina Ríos","username":"vrios","rol":"Comprador"},{"dni":37654321,"nombre":"Tomás Ibáñez","username":"tibanez","rol":"Comprador"},{"dni":42345678,"nombre":"Camila Suárez","username":"csuarez","rol":"Comprador"},{"dni":39876543,"nombre":"Agustín Molina","username":"amolina","rol":"Comprador"},{"dni":43456789,"nombre":"Julieta Torres","username":"jtorres","rol":"Comprador"},{"dni":36765432,"nombre":"Bruno Acosta","username":"bacosta","rol":"Comprador"}]
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 4. GET eventos (vacio)

```bash
curl http://localhost:5190/api/eventos
```

Respuesta:

```
[]
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 5. POST evento organizador (futuro con 2 modalidades)

```bash
curl -X POST "http://localhost:5190/api/eventos?dni=30111222" -H "Content-Type: application/json" -d '{"nombre":"Recital Rock","descripcion":"Banda local","fecha":"2026-12-15T21:00:00","lugar":"Club Atletico","modalidades":[{"nombre":"General","beneficios":"","precio":1000,"cupoTotal":100},{"nombre":"VIP","beneficios":"Consumicion","precio":3000,"cupoTotal":3}]}'
```

Respuesta:

```
{"id":1,"nombre":"Recital Rock","descripcion":"Banda local","fecha":"2026-12-15T21:00:00","lugar":"Club Atletico","cancelado":false,"modalidades":[{"id":1,"nombre":"General","beneficios":"","precio":1000,"cupoTotal":100,"cupoDisponible":100},{"id":2,"nombre":"VIP","beneficios":"Consumicion","precio":3000,"cupoTotal":3,"cupoDisponible":3}]}
HTTP 201
```

Resultado: **OK** (HTTP 201)

### 6. POST evento organizador (hoy, en 3 minutos: para probar luego fecha pasada)

```bash
curl -X POST "http://localhost:5190/api/eventos?dni=29888777" -H "Content-Type: application/json" -d '{"nombre":"Fiesta Hoy","descripcion":"Evento del dia","fecha":"2026-10-04T08:36:00","lugar":"Salon Centro","modalidades":[{"nombre":"General","beneficios":"","precio":500,"cupoTotal":50}]}'
```

Respuesta:

```
{"id":2,"nombre":"Fiesta Hoy","descripcion":"Evento del dia","fecha":"2026-10-04T08:36:00","lugar":"Salon Centro","cancelado":false,"modalidades":[{"id":1,"nombre":"General","beneficios":"","precio":500,"cupoTotal":50,"cupoDisponible":50}]}
HTTP 201
```

Resultado: **OK** (HTTP 201)

### 7. POST evento organizador (para cancelar)

```bash
curl -X POST "http://localhost:5190/api/eventos?dni=30111222" -H "Content-Type: application/json" -d '{"nombre":"Obra de teatro","descripcion":"x","fecha":"2026-11-20T20:00:00","lugar":"Teatro 25","modalidades":[{"nombre":"Platea","beneficios":"","precio":800,"cupoTotal":10}]}'
```

Respuesta:

```
{"id":3,"nombre":"Obra de teatro","descripcion":"x","fecha":"2026-11-20T20:00:00","lugar":"Teatro 25","cancelado":false,"modalidades":[{"id":1,"nombre":"Platea","beneficios":"","precio":800,"cupoTotal":10,"cupoDisponible":10}]}
HTTP 201
```

Resultado: **OK** (HTTP 201)

### 8. POST evento comprador (rechazo rol)

```bash
curl -X POST "http://localhost:5190/api/eventos?dni=40123456" -H "Content-Type: application/json" -d '{"nombre":"Hack","descripcion":"x","fecha":"2026-12-01T20:00:00","lugar":"X"}'
```

Respuesta:

```
El usuario no tiene permiso para esta acción.
HTTP 403
```

Resultado: **OK** (HTTP 403)

### 9. POST evento sin dni

```bash
curl -X POST http://localhost:5190/api/eventos -H "Content-Type: application/json" -d '{"nombre":"Hack","descripcion":"x","fecha":"2026-12-01T20:00:00","lugar":"X"}'
```

Respuesta:

```
El usuario no tiene permiso para esta acción.
HTTP 403
```

Resultado: **OK** (HTTP 403): sin DNI no se permite la acción.

### 10. POST evento dni inexistente

```bash
curl -X POST "http://localhost:5190/api/eventos?dni=11111111" -H "Content-Type: application/json" -d '{"nombre":"Hack","descripcion":"x","fecha":"2026-12-01T20:00:00","lugar":"X"}'
```

Respuesta:

```
El usuario no tiene permiso para esta acción.
HTTP 403
```

Resultado: **OK** (HTTP 403)

### 11. POST evento fecha pasada

```bash
curl -X POST "http://localhost:5190/api/eventos?dni=30111222" -H "Content-Type: application/json" -d '{"nombre":"Viejo","descripcion":"x","fecha":"2025-01-01T20:00:00","lugar":"X"}'
```

Respuesta:

```
La fecha debe ser futura.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 12. GET eventos

```bash
curl http://localhost:5190/api/eventos
```

Respuesta:

```
[{"id":1,"nombre":"Recital Rock","descripcion":"Banda local","fecha":"2026-12-15T21:00:00","lugar":"Club Atletico","cancelado":false,"modalidades":[{"id":1,"nombre":"General","beneficios":"","precio":1000,"cupoTotal":100,"cupoDisponible":100},{"id":2,"nombre":"VIP","beneficios":"Consumicion","precio":3000,"cupoTotal":3,"cupoDisponible":3}]},{"id":2,"nombre":"Fiesta Hoy","descripcion":"Evento del dia","fecha":"2026-10-04T08:36:00","lugar":"Salon Centro","cancelado":false,"modalidades":[{"id":1,"nombre":"General","beneficios":"","precio":500,"cupoTotal":50,"cupoDisponible":50}]},{"id":3,"nombre":"Obra de teatro","descripcion":"x","fecha":"2026-11-20T20:00:00","lugar":"Teatro 25","cancelado":false,"modalidades":[{"id":1,"nombre":"Platea","beneficios":"","precio":800,"cupoTotal":10,"cupoDisponible":10}]}]
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 13. GET evento 1

```bash
curl http://localhost:5190/api/eventos/1
```

Respuesta:

```
{"id":1,"nombre":"Recital Rock","descripcion":"Banda local","fecha":"2026-12-15T21:00:00","lugar":"Club Atletico","cancelado":false,"modalidades":[{"id":1,"nombre":"General","beneficios":"","precio":1000,"cupoTotal":100,"cupoDisponible":100},{"id":2,"nombre":"VIP","beneficios":"Consumicion","precio":3000,"cupoTotal":3,"cupoDisponible":3}]}
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 14. GET evento 999

```bash
curl http://localhost:5190/api/eventos/999
```

Respuesta:

```
{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.5","title":"Not Found","status":404,"traceId":"00-367545b337379dd00aff6facf7fd3a43-776af73a7a0986be-00"}
HTTP 404
```

Resultado: **OK** (HTTP 404)

### 15. PUT evento 1 organizador

```bash
curl -X PUT "http://localhost:5190/api/eventos/1?dni=30111222" -H "Content-Type: application/json" -d '{"nombre":"Recital Rock (editado)","descripcion":"Banda local + invitados","fecha":"2026-12-16T21:00:00","lugar":"Club Atletico"}'
```

Respuesta:

```
HTTP 204
```

Resultado: **OK** (HTTP 204)

### 16. PUT evento 1 comprador (rechazo)

```bash
curl -X PUT "http://localhost:5190/api/eventos/1?dni=40123456" -H "Content-Type: application/json" -d '{"nombre":"X","descripcion":"x","fecha":"2026-12-16T21:00:00","lugar":"X"}'
```

Respuesta:

```
El usuario no tiene permiso para esta acción.
HTTP 403
```

Resultado: **OK** (HTTP 403)

### 17. PUT evento 999 organizador

```bash
curl -X PUT "http://localhost:5190/api/eventos/999?dni=30111222" -H "Content-Type: application/json" -d '{"nombre":"X","descripcion":"x","fecha":"2026-12-16T21:00:00","lugar":"X"}'
```

Respuesta:

```
El evento no existe.
HTTP 400
```

Resultado: **OK con observación** (HTTP 400): se rechaza, pero un evento inexistente debería responder 404.

### 18. GET evento 1 tras edicion

```bash
curl http://localhost:5190/api/eventos/1
```

Respuesta:

```
{"id":1,"nombre":"Recital Rock (editado)","descripcion":"Banda local + invitados","fecha":"2026-12-16T21:00:00","lugar":"Club Atletico","cancelado":false,"modalidades":[{"id":1,"nombre":"General","beneficios":"","precio":1000,"cupoTotal":100,"cupoDisponible":100},{"id":2,"nombre":"VIP","beneficios":"Consumicion","precio":3000,"cupoTotal":3,"cupoDisponible":3}]}
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 19. POST modalidad evento 1 organizador

```bash
curl -X POST "http://localhost:5190/api/eventos/1/modalidades?dni=30111222" -H "Content-Type: application/json" -d '{"nombre":"Campo delantero","beneficios":"Cerca del escenario","precio":2000,"cupoTotal":20}'
```

Respuesta:

```
{"id":3,"nombre":"Campo delantero","beneficios":"Cerca del escenario","precio":2000,"cupoTotal":20,"cupoDisponible":20}
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 20. POST modalidad evento 1 comprador (rechazo)

```bash
curl -X POST "http://localhost:5190/api/eventos/1/modalidades?dni=40123456" -H "Content-Type: application/json" -d '{"nombre":"Y","beneficios":"","precio":1,"cupoTotal":1}'
```

Respuesta:

```
El usuario no tiene permiso para esta acción.
HTTP 403
```

Resultado: **OK** (HTTP 403)

### 21. POST modalidad cupo 0 (invalida)

```bash
curl -X POST "http://localhost:5190/api/eventos/1/modalidades?dni=30111222" -H "Content-Type: application/json" -d '{"nombre":"Z","beneficios":"","precio":1,"cupoTotal":0}'
```

Respuesta:

```
La modalidad debe tener nombre, precio no negativo y cupo mayor a cero.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 22. PUT cancelar evento 3 comprador (rechazo)

```bash
curl -X PUT "http://localhost:5190/api/eventos/3/cancelar?dni=40123456"
```

Respuesta:

```
El usuario no tiene permiso para esta acción.
HTTP 403
```

Resultado: **OK** (HTTP 403)

### 23. PUT cancelar evento 3 organizador

```bash
curl -X PUT "http://localhost:5190/api/eventos/3/cancelar?dni=30111222"
```

Respuesta:

```
HTTP 204
```

Resultado: **OK** (HTTP 204)

### 24. PUT cancelar evento 3 otra vez

```bash
curl -X PUT "http://localhost:5190/api/eventos/3/cancelar?dni=30111222"
```

Respuesta:

```
El evento ya está cancelado.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 25. GET eventos (no deberia listar el cancelado)

```bash
curl http://localhost:5190/api/eventos
```

Respuesta:

```
[{"id":1,"nombre":"Recital Rock (editado)","descripcion":"Banda local + invitados","fecha":"2026-12-16T21:00:00","lugar":"Club Atletico","cancelado":false,"modalidades":[{"id":1,"nombre":"General","beneficios":"","precio":1000,"cupoTotal":100,"cupoDisponible":100},{"id":2,"nombre":"VIP","beneficios":"Consumicion","precio":3000,"cupoTotal":3,"cupoDisponible":3},{"id":3,"nombre":"Campo delantero","beneficios":"Cerca del escenario","precio":2000,"cupoTotal":20,"cupoDisponible":20}]},{"id":2,"nombre":"Fiesta Hoy","descripcion":"Evento del dia","fecha":"2026-10-04T08:36:00","lugar":"Salon Centro","cancelado":false,"modalidades":[{"id":1,"nombre":"General","beneficios":"","precio":500,"cupoTotal":50,"cupoDisponible":50}]}]
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 26. POST compra 2 entradas general evento 1

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":40123456,"eventoId":1,"modalidadId":1,"cantidad":2}'
```

Respuesta:

```
{"id":1,"dniComprador":"40123456","fecha":"2026-10-04T08:33:20.1176802-03:00","total":2000,"entradas":[{"codigo":"000001","compraId":1,"eventoId":1,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null},{"codigo":"000002","compraId":1,"eventoId":1,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null}]}
HTTP 201
```

Resultado: **OK** (HTTP 201)

### 27. POST compra 5 entradas (descuento) evento 1

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":38456789,"eventoId":1,"modalidadId":1,"cantidad":5}'
```

Respuesta:

```
{"id":2,"dniComprador":"38456789","fecha":"2026-10-04T08:33:20.267412-03:00","total":4500.0,"entradas":[{"codigo":"000003","compraId":2,"eventoId":1,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null},{"codigo":"000004","compraId":2,"eventoId":1,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null},{"codigo":"000005","compraId":2,"eventoId":1,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null},{"codigo":"000006","compraId":2,"eventoId":1,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null},{"codigo":"000007","compraId":2,"eventoId":1,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null}]}
HTTP 201
```

Resultado: **OK** (HTTP 201)

### 28. POST compra organizador (rechazo)

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":30111222,"eventoId":1,"modalidadId":1,"cantidad":1}'
```

Respuesta:

```
El usuario no tiene permiso para esta acción.
HTTP 403
```

Resultado: **OK** (HTTP 403)

### 29. POST compra dni inexistente

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":12345678,"eventoId":1,"modalidadId":1,"cantidad":1}'
```

Respuesta:

```
El usuario no tiene permiso para esta acción.
HTTP 403
```

Resultado: **OK** (HTTP 403)

### 30. POST compra cupo agotado VIP (pide 4 de 3)

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":41234567,"eventoId":1,"modalidadId":2,"cantidad":4}'
```

Respuesta:

```
No queda cupo suficiente.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 31. POST compra VIP 3 (agota)

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":41234567,"eventoId":1,"modalidadId":2,"cantidad":3}'
```

Respuesta:

```
{"id":3,"dniComprador":"41234567","fecha":"2026-10-04T08:33:20.5846712-03:00","total":9000,"entradas":[{"codigo":"000008","compraId":3,"eventoId":1,"modalidadEntradaId":2,"usada":false,"fechaIngreso":null},{"codigo":"000009","compraId":3,"eventoId":1,"modalidadEntradaId":2,"usada":false,"fechaIngreso":null},{"codigo":"00000A","compraId":3,"eventoId":1,"modalidadEntradaId":2,"usada":false,"fechaIngreso":null}]}
HTTP 201
```

Resultado: **OK** (HTTP 201)

### 32. POST compra VIP 1 mas (agotado)

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":41234567,"eventoId":1,"modalidadId":2,"cantidad":1}'
```

Respuesta:

```
No queda cupo suficiente.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 33. POST compra evento cancelado

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":40123456,"eventoId":3,"modalidadId":1,"cantidad":1}'
```

Respuesta:

```
No se puede comprar para un evento cancelado o cuya fecha ya pasó.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 34. POST compra evento inexistente

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":40123456,"eventoId":99,"modalidadId":1,"cantidad":1}'
```

Respuesta:

```
El evento no existe.
HTTP 400
```

Resultado: **OK con observación** (HTTP 400): se rechaza, pero un evento inexistente debería responder 404.

### 35. POST compra cantidad 0

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":40123456,"eventoId":1,"modalidadId":1,"cantidad":0}'
```

Respuesta:

```
La cantidad debe ser mayor a cero.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 36. POST compra evento de hoy (2)

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":37654321,"eventoId":2,"modalidadId":1,"cantidad":2}'
```

Respuesta:

```
{"id":4,"dniComprador":"37654321","fecha":"2026-10-04T08:33:20.9412267-03:00","total":1000,"entradas":[{"codigo":"00000B","compraId":4,"eventoId":2,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null},{"codigo":"00000C","compraId":4,"eventoId":2,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null}]}
HTTP 201
```

Resultado: **OK** (HTTP 201)

### 37. GET compra 1

```bash
curl http://localhost:5190/api/compras/1
```

Respuesta:

```
{"id":1,"dniComprador":"40123456","fecha":"2026-10-04T08:33:20.1176802-03:00","total":2000,"entradas":[{"codigo":"000001","compraId":1,"eventoId":1,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null},{"codigo":"000002","compraId":1,"eventoId":1,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null}]}
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 38. GET compra 999

```bash
curl http://localhost:5190/api/compras/999
```

Respuesta:

```
{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.5","title":"Not Found","status":404,"traceId":"00-c17f1a05d21dce88835cb7222879e43b-d3c6dc9cbfeef48e-00"}
HTTP 404
```

Resultado: **OK** (HTTP 404)

### 39. GET reporte organizador

```bash
curl http://localhost:5190/api/reportes/recaudacion?dni=29888777
```

Respuesta:

```
[{"eventoId":1,"nombre":"Recital Rock (editado)","entradasVendidas":10,"recaudacion":15500.0},{"eventoId":2,"nombre":"Fiesta Hoy","entradasVendidas":2,"recaudacion":1000},{"eventoId":3,"nombre":"Obra de teatro","entradasVendidas":0,"recaudacion":0}]
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 40. GET reporte comprador (rechazo)

```bash
curl http://localhost:5190/api/reportes/recaudacion?dni=40123456
```

Respuesta:

```
El usuario no tiene permiso para esta acción.
HTTP 403
```

Resultado: **OK** (HTTP 403)

### 41. GET reporte sin dni

```bash
curl http://localhost:5190/api/reportes/recaudacion
```

Respuesta:

```
El usuario no tiene permiso para esta acción.
HTTP 403
```

Resultado: **OK** (HTTP 403)

### 42. Swagger validacion

```bash
curl -o /dev/null http://localhost:5191/swagger/index.html
```

Respuesta:

```
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 43. POST evento organizador (hoy 04/10 23:00, para validar en puerta)

```bash
curl -X POST "http://localhost:5190/api/eventos?dni=30111222" -H "Content-Type: application/json" -d '{"nombre":"Fiesta Domingo","descripcion":"Evento del dia","fecha":"2026-10-04T23:00:00","lugar":"Salon Centro","modalidades":[{"nombre":"General","beneficios":"","precio":500,"cupoTotal":50}]}'
```

Respuesta:

```
{"id":4,"nombre":"Fiesta Domingo","descripcion":"Evento del dia","fecha":"2026-10-04T23:00:00","lugar":"Salon Centro","cancelado":false,"modalidades":[{"id":1,"nombre":"General","beneficios":"","precio":500,"cupoTotal":50,"cupoDisponible":50}]}
HTTP 201
```

Resultado: **OK** (HTTP 201)

### 44. POST compra 3 entradas evento 4 (desde gestion)

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":42345678,"eventoId":4,"modalidadId":1,"cantidad":3}'
```

Respuesta:

```
{"id":5,"dniComprador":"42345678","fecha":"2026-10-04T08:33:38.2708811-03:00","total":1500,"entradas":[{"codigo":"00000D","compraId":5,"eventoId":4,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null},{"codigo":"00000E","compraId":5,"eventoId":4,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null},{"codigo":"00000F","compraId":5,"eventoId":4,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null}]}
HTTP 201
```

Resultado: **OK** (HTTP 201)

### 45. POST validacion inmediata de entrada recien vendida (00000D, evento 4)

```bash
curl -X POST http://localhost:5191/api/validaciones -H "Content-Type: application/json" -d '{"codigo":"00000D","eventoId":4}'
```

Respuesta:

```
{"codigo":"00000D","compraId":5,"eventoId":4,"modalidadEntradaId":1,"usada":true,"fechaIngreso":"2026-10-04T08:33:38.6579372-03:00"}
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 46. POST revalidar la misma entrada

```bash
curl -X POST http://localhost:5191/api/validaciones -H "Content-Type: application/json" -d '{"codigo":"00000D","eventoId":4}'
```

Respuesta:

```
La entrada ya fue usada.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 47. POST validar otra entrada de la misma compra (00000E)

```bash
curl -X POST http://localhost:5191/api/validaciones -H "Content-Type: application/json" -d '{"codigo":"00000E","eventoId":4}'
```

Respuesta:

```
{"codigo":"00000E","compraId":5,"eventoId":4,"modalidadEntradaId":1,"usada":true,"fechaIngreso":"2026-10-04T08:33:38.8336877-03:00"}
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 48. GET compra 5 desde gestion (refleja ingresos hechos en validacion)

```bash
curl http://localhost:5190/api/compras/5
```

Respuesta:

```
{"id":5,"dniComprador":"42345678","fecha":"2026-10-04T08:33:38.2708811-03:00","total":1500,"entradas":[{"codigo":"00000D","compraId":5,"eventoId":4,"modalidadEntradaId":1,"usada":true,"fechaIngreso":"2026-10-04T08:33:38.6579372-03:00"},{"codigo":"00000E","compraId":5,"eventoId":4,"modalidadEntradaId":1,"usada":true,"fechaIngreso":"2026-10-04T08:33:38.8336877-03:00"},{"codigo":"00000F","compraId":5,"eventoId":4,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null}]}
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 49. POST validar codigo inexistente

```bash
curl -X POST http://localhost:5191/api/validaciones -H "Content-Type: application/json" -d '{"codigo":"ZZZZZZ","eventoId":4}'
```

Respuesta:

```
La entrada no existe.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 50. POST validar entrada de otro evento (00000F es del 4, se presenta en el 1)

```bash
curl -X POST http://localhost:5191/api/validaciones -H "Content-Type: application/json" -d '{"codigo":"00000F","eventoId":1}'
```

Respuesta:

```
La entrada es de otro evento.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 51. POST validar entrada de evento futuro (000001, evento 1)

```bash
curl -X POST http://localhost:5191/api/validaciones -H "Content-Type: application/json" -d '{"codigo":"000001","eventoId":1}'
```

Respuesta:

```
La entrada solo se puede usar el día del evento.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 52. POST validar sin eventoId

```bash
curl -X POST http://localhost:5191/api/validaciones -H "Content-Type: application/json" -d '{"codigo":"00000F"}'
```

Respuesta:

```
La entrada es de otro evento.
HTTP 400
```

Resultado: **OK con observación** (HTTP 400): se rechaza, pero el motivo informado ("otro evento") no corresponde: faltó el dato del evento.

### 53. POST validar codigo en minusculas (00000f)

```bash
curl -X POST http://localhost:5191/api/validaciones -H "Content-Type: application/json" -d '{"codigo":"00000f","eventoId":4}'
```

Respuesta:

```
La entrada no existe.
HTTP 400
```

Resultado: **OK con observación** (HTTP 400): el código distingue mayúsculas de minúsculas; convendría normalizarlo.

### 54. POST validar codigo vacio

```bash
curl -X POST http://localhost:5191/api/validaciones -H "Content-Type: application/json" -d '{"codigo":"","eventoId":4}'
```

Respuesta:

```
La entrada no existe.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 55. POST validar body vacio

```bash
curl -X POST http://localhost:5191/api/validaciones -H "Content-Type: application/json" -d '{}'
```

Respuesta:

```
La entrada no existe.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 56. POST validar entrada del evento 2 (hoy 08:36) antes de que empiece (00000B)

```bash
curl -X POST http://localhost:5191/api/validaciones -H "Content-Type: application/json" -d '{"codigo":"00000B","eventoId":2}'
```

Respuesta:

```
{"codigo":"00000B","compraId":4,"eventoId":2,"modalidadEntradaId":1,"usada":true,"fechaIngreso":"2026-10-04T08:33:50.0054421-03:00"}
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 57. POST evento organizador (hoy 22:00, para cancelar luego de vender)

```bash
curl -X POST "http://localhost:5190/api/eventos?dni=30111222" -H "Content-Type: application/json" -d '{"nombre":"Show Cancelable","descripcion":"x","fecha":"2026-10-04T22:00:00","lugar":"Anfiteatro","modalidades":[{"nombre":"General","beneficios":"","precio":700,"cupoTotal":20}]}'
```

Respuesta:

```
{"id":5,"nombre":"Show Cancelable","descripcion":"x","fecha":"2026-10-04T22:00:00","lugar":"Anfiteatro","cancelado":false,"modalidades":[{"id":1,"nombre":"General","beneficios":"","precio":700,"cupoTotal":20,"cupoDisponible":20}]}
HTTP 201
```

Resultado: **OK** (HTTP 201)

### 58. POST compra 1 entrada evento 5

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":39876543,"eventoId":5,"modalidadId":1,"cantidad":1}'
```

Respuesta:

```
{"id":6,"dniComprador":"39876543","fecha":"2026-10-04T08:33:50.1984242-03:00","total":700,"entradas":[{"codigo":"00000G","compraId":6,"eventoId":5,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null}]}
HTTP 201
```

Resultado: **OK** (HTTP 201)

### 59. PUT cancelar evento 5 (con entradas vendidas)

```bash
curl -X PUT "http://localhost:5190/api/eventos/5/cancelar?dni=29888777"
```

Respuesta:

```
HTTP 204
```

Resultado: **OK** (HTTP 204)

### 60. POST validar entrada de evento cancelado (00000G, evento 5)

```bash
curl -X POST http://localhost:5191/api/validaciones -H "Content-Type: application/json" -d '{"codigo":"00000G","eventoId":5}'
```

Respuesta:

```
El evento no existe o está cancelado.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 61. Concurrencia: 10 compras simultáneas en gestión (2 entradas c/u, evento 4) + 2 validaciones simultáneas en validación (00000F evento 4, 00000C evento 2)

```bash
curl (x10 en paralelo) -X POST http://localhost:5190/api/compras -H Content-Type:application/json -d {"dni":36765432,"eventoId":4,"modalidadId":1,"cantidad":2}  &  curl (x2 en paralelo) -X POST http://localhost:5191/api/validaciones ...
```

Respuesta:

```
10 compras -> HTTP 201 (ids 7 a 16, sin repetidos); 20 códigos generados, 0 repetidos
{"codigo":"00000F","compraId":5,"eventoId":4,"modalidadEntradaId":1,"usada":true,"fechaIngreso":"2026-10-04T08:33:58.5932751-03:00"}
{"codigo":"00000C","compraId":4,"eventoId":2,"modalidadEntradaId":1,"usada":true,"fechaIngreso":"2026-10-04T08:33:58.6404686-03:00"}
HTTP 200
```

Resultado: **OK**: todas las operaciones concurrentes respondieron bien y no hubo pérdida de datos ni códigos repetidos.

### 62. GET evento 4 luego de la concurrencia (50 - 3 - 20 = 27)

```bash
curl http://localhost:5190/api/eventos/4
```

Respuesta:

```
... "cupoDisponible":27 ...
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 63. POST compra evento con fecha pasada (evento 2, empezó a las 08:36)

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":40123456,"eventoId":2,"modalidadId":1,"cantidad":1}'
```

Respuesta:

```
No se puede comprar para un evento cancelado o cuya fecha ya pasó.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 64. GET eventos (el evento 2 ya no aparece, el 5 cancelado tampoco)

```bash
curl http://localhost:5190/api/eventos
```

Respuesta:

```
[{"id":1,"nombre":"Recital Rock (editado)","descripcion":"Banda local + invitados","fecha":"2026-12-16T21:00:00","lugar":"Club Atletico","cancelado":false,"modalidades":[{"id":1,"nombre":"General","beneficios":"","precio":1000,"cupoTotal":100,"cupoDisponible":93},{"id":2,"nombre":"VIP","beneficios":"Consumicion","precio":3000,"cupoTotal":3,"cupoDisponible":0},{"id":3,"nombre":"Campo delantero","beneficios":"Cerca del escenario","precio":2000,"cupoTotal":20,"cupoDisponible":20}]},{"id":4,"nombre":"Fiesta Domingo","descripcion":"Evento del dia","fecha":"2026-10-04T23:00:00","lugar":"Salon Centro","cancelado":false,"modalidades":[{"id":1,"nombre":"General","beneficios":"","precio":500,"cupoTotal":50,"cupoDisponible":27}]}]
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 65. GET compra 4 (entradas del evento 2 validadas)

```bash
curl http://localhost:5190/api/compras/4
```

Respuesta:

```
{"id":4,"dniComprador":"37654321","fecha":"2026-10-04T08:33:20.9412267-03:00","total":1000,"entradas":[{"codigo":"00000B","compraId":4,"eventoId":2,"modalidadEntradaId":1,"usada":true,"fechaIngreso":"2026-10-04T08:33:50.0054421-03:00"},{"codigo":"00000C","compraId":4,"eventoId":2,"modalidadEntradaId":1,"usada":true,"fechaIngreso":"2026-10-04T08:33:58.6404686-03:00"}]}
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 66. Persistencia: (tras matar y reiniciar ambas APIs) GET compra 5

```bash
curl http://localhost:5190/api/compras/5
```

Respuesta:

```
{"id":5,"dniComprador":"42345678","fecha":"2026-10-04T08:33:38.2708811-03:00","total":1500,"entradas":[{"codigo":"00000D","compraId":5,"eventoId":4,"modalidadEntradaId":1,"usada":true,"fechaIngreso":"2026-10-04T08:33:38.6579372-03:00"},{"codigo":"00000E","compraId":5,"eventoId":4,"modalidadEntradaId":1,"usada":true,"fechaIngreso":"2026-10-04T08:33:38.8336877-03:00"},{"codigo":"00000F","compraId":5,"eventoId":4,"modalidadEntradaId":1,"usada":true,"fechaIngreso":"2026-10-04T08:33:58.5932751-03:00"}]}
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 67. Persistencia: (tras reiniciar) revalidar 00000E

```bash
curl -X POST http://localhost:5191/api/validaciones -H "Content-Type: application/json" -d '{"codigo":"00000E","eventoId":4}'
```

Respuesta:

```
La entrada ya fue usada.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 68. Persistencia: (tras reiniciar) POST compra 1 entrada evento 4 (el contador de codigos continua)

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":43456789,"eventoId":4,"modalidadId":1,"cantidad":1}'
```

Respuesta:

```
{"id":17,"dniComprador":"43456789","fecha":"2026-10-04T08:36:42.8191225-03:00","total":500,"entradas":[{"codigo":"000011","compraId":17,"eventoId":4,"modalidadEntradaId":1,"usada":false,"fechaIngreso":null}]}
HTTP 201
```

Resultado: **OK** (HTTP 201)

### 69. Persistencia: (tras reiniciar) validar 00000Z (entrada de una de las compras concurrentes, nunca validada)

```bash
curl -X POST http://localhost:5191/api/validaciones -H "Content-Type: application/json" -d '{"codigo":"00000Z","eventoId":4}'
```

Respuesta:

```
{"codigo":"00000Z","compraId":16,"eventoId":4,"modalidadEntradaId":1,"usada":true,"fechaIngreso":"2026-10-04T08:36:42.8731349-03:00"}
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 70. (tras reiniciar) GET reporte organizador

```bash
curl http://localhost:5190/api/reportes/recaudacion?dni=30111222
```

Respuesta:

```
[{"eventoId":1,"nombre":"Recital Rock (editado)","entradasVendidas":10,"recaudacion":15500.0},{"eventoId":2,"nombre":"Fiesta Hoy","entradasVendidas":2,"recaudacion":1000},{"eventoId":3,"nombre":"Obra de teatro","entradasVendidas":0,"recaudacion":0},{"eventoId":4,"nombre":"Fiesta Domingo","entradasVendidas":24,"recaudacion":12000},{"eventoId":5,"nombre":"Show Cancelable","entradasVendidas":1,"recaudacion":700}]
HTTP 200
```

Resultado: **OK con observación** (HTTP 200): el reporte incluye la venta del evento 5, que fue cancelado, sin indicar su estado.

### 71. GET evento 3 cancelado (detalle)

```bash
curl http://localhost:5190/api/eventos/3
```

Respuesta:

```
{"id":3,"nombre":"Obra de teatro","descripcion":"x","fecha":"2026-11-20T20:00:00","lugar":"Teatro 25","cancelado":true,"modalidades":[{"id":1,"nombre":"Platea","beneficios":"","precio":800,"cupoTotal":10,"cupoDisponible":10}]}
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 72. POST compra modalidad inexistente

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{"dni":43456789,"eventoId":1,"modalidadId":9,"cantidad":1}'
```

Respuesta:

```
La modalidad no existe.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 73. POST evento con DNI en header (no soportado: el grupo usa query string)

```bash
curl -X POST http://localhost:5190/api/eventos -H "dni: 30111222" -H "Content-Type: application/json" -d '{"nombre":"H","descripcion":"x","fecha":"2026-12-01T20:00:00","lugar":"X"}'
```

Respuesta:

```
El usuario no tiene permiso para esta acción.
HTTP 403
```

Resultado: **OK** (HTTP 403): prueba informativa; el grupo eligió recibir el DNI por query string, el header no se lee.

### 74. PUT editar evento cancelado 3

```bash
curl -X PUT "http://localhost:5190/api/eventos/3?dni=30111222" -H "Content-Type: application/json" -d '{"nombre":"X","descripcion":"x","fecha":"2026-12-16T21:00:00","lugar":"X"}'
```

Respuesta:

```
No se puede editar un evento cancelado.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 75. POST modalidad a evento cancelado 3

```bash
curl -X POST "http://localhost:5190/api/eventos/3/modalidades?dni=30111222" -H "Content-Type: application/json" -d '{"nombre":"Y","beneficios":"","precio":1,"cupoTotal":1}'
```

Respuesta:

```
No se puede agregar una modalidad a un evento cancelado.
HTTP 400
```

Resultado: **OK** (HTTP 400)

### 76. POST compra con body vacio

```bash
curl -X POST http://localhost:5190/api/compras -H "Content-Type: application/json" -d '{}'
```

Respuesta:

```
El usuario no tiene permiso para esta acción.
HTTP 403
```

Resultado: **OK con observación** (HTTP 403): se rechaza, aunque un pedido sin datos debería responder 400.

### 77. Persistencia: (tras reiniciar) validar la entrada recien comprada 000011

```bash
curl -X POST http://localhost:5191/api/validaciones -H "Content-Type: application/json" -d '{"codigo":"000011","eventoId":4}'
```

Respuesta:

```
{"codigo":"000011","compraId":17,"eventoId":4,"modalidadEntradaId":1,"usada":true,"fechaIngreso":"2026-10-04T08:36:50.6114322-03:00"}
HTTP 200
```

Resultado: **OK** (HTTP 200)

### 78. GET compra 17 desde gestion (refleja el ingreso)

```bash
curl http://localhost:5190/api/compras/17
```

Respuesta:

```
{"id":17,"dniComprador":"43456789","fecha":"2026-10-04T08:36:42.8191225-03:00","total":500,"entradas":[{"codigo":"000011","compraId":17,"eventoId":4,"modalidadEntradaId":1,"usada":true,"fechaIngreso":"2026-10-04T08:36:50.6114322-03:00"}]}
HTTP 200
```

Resultado: **OK** (HTTP 200)

## Resumen

**Endpoints obligatorios de la sección 7 (11): 11 funcionan, 0 fallan.**

| Endpoint | Resultado |
|---|---|
| GET `/api/usuarios` | OK — devuelve los 10 usuarios del anexo |
| GET `/api/eventos` | OK — lista solo eventos vigentes (no cancelados y con fecha futura) |
| GET `/api/eventos/{id}` | OK — detalle con modalidades; 404 si no existe |
| POST `/api/eventos` | OK — 201 con organizador; 403 con comprador, DNI inexistente o sin DNI; 400 con fecha pasada |
| PUT `/api/eventos/{id}` | OK — 204 con organizador; 403 con comprador; rechaza editar un evento cancelado |
| PUT `/api/eventos/{id}/cancelar` | OK — 204 con organizador; 403 con comprador; rechaza cancelar dos veces |
| POST `/api/eventos/{id}/modalidades` | OK — 200 con organizador; 403 con comprador; valida cupo y precio; rechaza evento cancelado |
| POST `/api/compras` | OK — genera una entrada por unidad; descuento del 10% desde 5 entradas; rechaza organizador, DNI inexistente, cupo agotado, evento cancelado, evento con fecha pasada, evento o modalidad inexistente y cantidad 0 |
| GET `/api/compras/{id}` | OK — muestra cada entrada con `usada` y `fechaIngreso`; 404 si no existe |
| GET `/api/reportes/recaudacion` | OK — recaudación y entradas vendidas por evento; 403 con comprador o sin DNI |
| POST `/api/validaciones` (API de validación) | OK — ver detalle abajo |

Validación y datos compartidos (foco de esta prueba):

- **Validación inmediata:** una entrada vendida desde gestión se valida desde la API de validación en el mismo segundo (pruebas 44-45). El detalle de la compra en gestión refleja enseguida `usada: true` y la `fechaIngreso` registrada por validación (pruebas 48, 65 y 78).
- **Validación individual:** de una compra de 3 entradas se validan dos y la tercera sigue sin usar (prueba 48).
- **Rechazos:** entrada ya usada (46 y 67, esta última después de reiniciar), código inexistente (49), entrada de otro evento (50), evento cancelado después de la venta (60) y evento de otro día (51).
- **Concurrencia entre las dos APIs:** 10 compras simultáneas en gestión junto con 2 validaciones simultáneas en validación terminaron todas bien. Quedaron 20 códigos sin repetir, ids de compra sin repetir y el cupo descontado correctamente (pruebas 61-62). El bloqueo del archivo `datos.lock` funciona.
- **Persistencia:** después de matar y volver a levantar las dos APIs se mantienen las compras, el estado de las entradas y el contador de códigos (la compra siguiente continuó en `000011`). Las entradas vendidas antes del reinicio se siguen pudiendo validar (pruebas 66-69 y 77-78).
- **Otros controles:**
  - Swagger de gestión responde (200); la API de validación también expone Swagger.
  - `usuarios.json` tiene exactamente los 10 usuarios del anexo.
  - El DELETE de entradas no corresponde a este grupo (ningún integrante en condición de promoción).
- **Códigos de entrada:** son de 6 caracteres alfanuméricos (0-9 y A-Z) y no se repiten, ni siquiera con concurrencia, pero son **secuenciales y predecibles** (`000001`, `000002`, ..., `00000A`, ..., `000011`).

Observaciones de las pruebas (no son fallas de funcionamiento):

- Los errores de reglas de negocio vuelven como texto plano con 400, incluso cuando el recurso no existe (editar el evento 999, comprar para el evento 99); en esos casos correspondería 404.
- En la validación, si no se envía `eventoId`, el rechazo informa "La entrada es de otro evento", que no es el motivo real. Un código vacío o un body vacío se informa como "La entrada no existe".
- El código de validación distingue mayúsculas de minúsculas (`00000f` no se encuentra).
- La validación solo se acepta el mismo día del evento (regla agregada por el grupo); se puede validar antes de la hora de inicio y también después.
- El reporte de recaudación suma las ventas de un evento cancelado (evento 5) sin distinguir su estado.
