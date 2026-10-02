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
{ "codigo": "000001", "eventoId": 1 }
```

La validación se permite el día del evento y registra el ingreso. Un segundo intento se rechaza. El detalle de la compra muestra el estado actualizado de cada entrada.
