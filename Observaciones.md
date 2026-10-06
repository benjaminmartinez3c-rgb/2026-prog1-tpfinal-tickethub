# Observaciones — 1ra entrega TP Final (Web APIs)

**Grupo:** tickethub
**Integrantes:** Benjamin Martinez, Santiago Pettinati
**Fecha de entrega evaluada:** 02/10/2026 (último commit `cc9b109`, 02/10/2026 00:09). No hay commits posteriores a la fecha de entrega.

## Estado general

Las dos soluciones compilan sin errores ni advertencias, los dos proyectos de tests corren en verde (14 tests en gestión y 8 en validación) y las dos APIs levantan. Probé todos los endpoints de la sección 7 con curl (el detalle está en `pruebas-profesor.md`): los 11 funcionan y se cumplen las reglas de negocio de la sección 5. Levanté las dos APIs desde el repositorio y los datos se comparten bien:
- una entrada vendida desde gestión se valida en el momento desde validación;
- el detalle de la compra en gestión refleja enseguida el ingreso;
- con compras y validaciones simultáneas en las dos APIs no se perdieron datos ni se repitieron códigos;
- todo sigue ahí después de reiniciar las APIs.

Funcionalmente, la entrega cumple lo pedido para el 02/10. Lo que sigue son ajustes de diseño y calidad que tienen que estar resueltos para el coloquio, y un punto importante sobre el trabajo en equipo en Git.

## Estructura y compilación

- La raíz del repositorio tiene `ApiGestion`, `ApiValidacion` y `Datos`. Falta la carpeta del frontend. No se penaliza porque el frontend se entrega el 16/10, pero tiene que aparecer en la raíz como tercera carpeta.
- Quedaron los archivos `Class1.cs` vacíos de la plantilla en `Gestion.Logica` y en `Validacion.Logica`. También quedó `Gestion.Api.http` apuntando a `/weatherforecast`. Hay que borrarlos.
- En `Validacion.Logica.csproj` la referencia a `Gestion.Logica` está duplicada (dos `ItemGroup` iguales en la misma línea). Hay que dejar una sola.
- El README indica los puertos 5175 y 5012 de `launchSettings`. Falta aclarar que hace falta el runtime de .NET 8. En una máquina que solo tiene .NET 10, las APIs no arrancan si no se usa la variable `DOTNET_ROLL_FORWARD`. Esa aclaración estaba en el README y se borró en el último commit.

## Endpoints

- Están los 11 endpoints obligatorios y responden correctamente. El DELETE de entradas no corresponde a este grupo.
- Revisar los códigos HTTP. Cuando el recurso no existe (editar o cancelar un evento inexistente, comprar para un evento o una modalidad inexistentes) la API responde 400 y debería responder 404. Un pedido sin datos (body vacío) responde 403 en lugar de 400.
- Los errores vuelven como texto plano. Conviene devolver un objeto con un mensaje, como vimos en la unidad 6, para que el frontend lo pueda mostrar de manera uniforme.
- En la validación, si no llega el evento, el motivo de rechazo dice "es de otro evento". El mensaje no corresponde: hay que validar que el dato venga y avisar que falta. Lo mismo pasa con un código vacío, que se informa como "la entrada no existe".
- El reporte de recaudación suma las ventas de los eventos cancelados sin indicar que están cancelados. Hay que definir qué se espera mostrar en ese caso.
- Los endpoints reciben y devuelven las entidades de la lógica (`Evento`, `ModalidadEntrada`, `Compra`, `Entrada`). Según lo visto en la unidad 6, la API tiene que trabajar con DTOs de request y response, con su mapeo en el proyecto de la API. Así, por ejemplo, el cliente no puede mandar `id`, `cancelado` o `cupoDisponible` al crear un evento o una modalidad.

## Reglas de negocio

- Se cumplen: cupo por modalidad, rechazo de eventos cancelados o con fecha pasada, descuento del 10% desde 5 entradas, una entrada por unidad comprada con validación individual, rechazo de una entrada ya usada, de una inexistente y de una de otro evento.
- El grupo agregó que una entrada solo se valida el mismo día del evento. Es razonable, pero tienen que poder explicar por qué lo decidieron y qué pasa con un evento que cruza la medianoche.
- **Códigos de entrada:** son de 6 caracteres alfanuméricos y no se repiten, pero son secuenciales (000001, 000002, ...). Cualquiera que tenga una entrada puede adivinar las de los demás, y para una entrada que se presenta en la puerta eso es un problema de seguridad. Hay que generar códigos que no se puedan predecir y seguir garantizando que no se repitan, por ejemplo controlando contra los ya emitidos.
- El código de validación distingue mayúsculas de minúsculas. Conviene normalizarlo, porque en la puerta lo va a tipear una persona.
- El listado de eventos devuelve solo los vigentes. Para el catálogo está bien, pero el organizador no tiene cómo ver los eventos cancelados o pasados salvo por id. Hay que evaluar si hace falta para la pantalla de gestión.

## Persistencia

- La persistencia está en archivos JSON dentro de la carpeta `Datos`, compartida entre las dos APIs y con ruta relativa al proyecto. Funciona y sobrevive a los reinicios.
- La clase que lee y escribe los archivos (`ArchivoDatos`) es una sola para todo el sistema y guarda todo el estado en un único archivo. Según la estructura vista en la unidad 5 y la 6, conviene tener repositorios por entidad, con la responsabilidad de acceso a datos separada de los servicios.
- El acceso al archivo con bloqueo exclusivo, reintentos y escritura en un archivo temporal no es algo que hayamos visto en clase. Si lo mantienen, tienen que poder explicarlo en detalle en el coloquio.

## Roles

- La validación por rol funciona: el DNI se recibe por query string en los endpoints de organizador y en el body en la compra, y se valida contra `usuarios.json`. Se rechaza al comprador, al DNI inexistente y al pedido sin DNI.
- El rol se maneja como texto. Corresponde modelarlo con un enum, como vimos en la unidad 2.
- La clase `Usuario` está en la carpeta `Entidades` pero declarada en otro namespace que el resto de las entidades. Hay que unificarlo.

## Tests

- Los dos proyectos de lógica tienen su propio proyecto de tests NUnit y los dos están en verde. Cubren casos que tienen que funcionar y casos que tienen que rechazarse: cupo, evento cancelado, evento pasado, descuento, permisos, persistencia, revalidación, entrada inexistente y de otro evento.
- Todos los rechazos se verifican con la excepción genérica. Cuando tengan excepciones propias (ver el punto siguiente), los tests tienen que verificar el tipo de excepción concreto.
- Falta probar el límite del descuento (4 entradas sin descuento y 5 con descuento) y que los códigos generados sean distintos entre compras.
- Los tests están todos en `UnitTest1.cs` con la clase `Tests`. Hay que renombrarlos según lo que prueban (por ejemplo, uno por servicio) y separar Arrange, Act y Assert.

## Calidad POO y capas

- Todas las reglas lanzan `Exception` genérica y el controller distingue los casos por tipo solo para el permiso. Corresponde definir excepciones propias (entrada inexistente, entrada ya usada, cupo agotado, evento no disponible, etc.) para que el controller pueda devolver el código HTTP adecuado en cada caso.
- No se usa inyección de dependencias: cada controller arma la ruta de los datos, crea el acceso a archivos y crea los servicios en cada método. Según la unidad 6, los servicios se registran en `Program.cs` y se reciben en el constructor del controller.
- El proyecto `Validacion.Logica` no tiene entidades ni acceso a datos propios: depende directamente de `Gestion.Logica` (otro proyecto de otra solución). Funciona, pero la validación queda acoplada a toda la lógica de gestión. Tienen que poder justificar la decisión o separar lo que realmente se comparte.
- Las entidades son clases de datos con todas las propiedades públicas y modificables, y todo el comportamiento está en los servicios. Por ejemplo, marcar una entrada como usada, calcular el total con descuento o descontar cupo son responsabilidades que podrían estar en la entidad que tiene esos datos. El enunciado pide pensar y defender dónde vive cada comportamiento.
- Las propiedades se inicializan en la misma declaración. Según los criterios de la cátedra, la inicialización va en el constructor.
- El DNI del comprador se guarda como texto en `Compra` y como número en `Usuario`. Hay que unificar el tipo.
- `ReporteEvento` está en la carpeta `Entidades` pero no es una entidad del dominio, sino un resultado de consulta.
- Falta configurar CORS en las dos APIs. Va a hacer falta para el frontend del 16/10.

## Git y trabajo en equipo

- El historial tiene 7 commits en total y en dos fechas: el 01/09 (estructura inicial y entidades) y el 02/10 entre las 00:07 y las 00:09. Casi todo el sistema (controllers, servicios, persistencia, tests de las dos APIs y README) entró en un único commit de 874 líneas la madrugada del día de entrega.
- El enunciado pide commits distribuidos a lo largo de las seis semanas y que cada integrante trabaje en más de una parte del sistema. El historial no permite ver eso. Además, el usuario de GitHub de Santiago Pettinati no aparece. Hay commits con el nombre "Benjamin Martinez" y el email de Santiago, y otros con el nombre y el email de Benjamin. Tienen que configurar correctamente nombre y email de Git cada uno en su máquina.
- Desde ahora tienen que trabajar con commits chicos y frecuentes, cada uno con su usuario, y en lo posible con ramas y pull requests. En el coloquio cada integrante va a tener que explicar cualquier parte del código.

## README

- El README tiene los comandos para levantar las APIs y ejemplos de uso, pero en el último commit se eliminaron el título, la explicación de la carpeta `Datos`, los comandos para correr los tests y la descripción de la organización del proyecto. Hay que volver a incluir al menos el nombre del proyecto, los integrantes, cómo correr los tests y cómo se comparten los datos.
- Para la próxima entrega hay que agregar cómo abrir el frontend.

## Pendiente para próximas entregas (no se evalúa ahora)

- Frontend (16/10): pantallas 1 a 3. Las pantallas 4 y 5 no son obligatorias para un equipo de dos personas.
- Video de la demo.

## Nota

**Aprobada**

Compila, las dos APIs levantan, los 11 endpoints funcionan, se cumplen las reglas de negocio y hay tests de las dos lógicas en verde. Para el coloquio tienen que resolver los códigos de entrada predecibles, los DTOs, las excepciones propias y la inyección de dependencias, y cada integrante tiene que poder defender el código, dado que el historial de commits no muestra trabajo distribuido.
