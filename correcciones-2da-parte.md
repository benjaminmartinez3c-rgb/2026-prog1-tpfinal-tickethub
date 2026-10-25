# Correcciones 2da entrega TP Final (Frontend) - tickethub

**Integrantes:** Benjamin Martinez y Santiago Pettinati (equipo de 2 personas)
**Fecha de corrección:** 24/10/2026
**Qué se evaluó:** la rama `main` hasta el último commit entregado en término, `fa430c7` ("Corregir fechas y completar DTOs, inyección de servicios y frontend", 16/10/2026 23:29). No hay commits posteriores al 16/10.

Como son dos personas, las pantallas 4 (control de acceso) y 5 (consulta de una compra) no son obligatorias. Igual hicieron la 4, así que también la probé. Ningún integrante está en condición de promoción, así que la sección 12 (Google Maps y cancelación de entradas) no aplica.

Cada punto a corregir tiene una etiqueta:
- **[Obligatorio para el coloquio]**: lo tienen que traer resuelto sí o sí.
- **[Recomendado]**: suma, pero no impide rendir.

---

## Estado general

Las dos soluciones compilan sin advertencias. Los tests están en verde: 19/19 en gestión y 9/9 en validación. Las dos APIs levantan con los comandos del README, y Swagger está en las dos.

El frontend es una sola página (`Frontend/index.html`, `script.js` y `style.css`), en HTML, CSS y JavaScript sin frameworks. Cubre las pantallas 1, 2 y 3, y además agrega una sección de validación (pantalla 4). Hice el recorrido completo desde el navegador y funciona:
1. Elegí un usuario.
2. Abrí el catálogo y el detalle de un evento.
3. Compré varias entradas, y el descuento se aplicó desde 5.
4. Validé una de esas entradas en la puerta.
5. Intenté validarla de nuevo y la rechazó.

Lo que falta o hay que arreglar para el coloquio:
- **Accesos simultáneos al archivo de datos.** En el último commit se sacó el bloqueo que tenían en la 1ra entrega. Con dos pedidos al mismo tiempo, se pierden compras y validaciones, se repiten ids y una entrada puede usarse dos veces. Se reproduce con un doble clic en "Comprar".
- **Video de la demo.** No está.

El resto son mejoras de usabilidad y de calidad.

---

## Checklist de entregables

| Ítem | Estado | ¿Para el coloquio? | Comentario |
|---|---|---|---|
| Pantalla 1 – Elegir usuario | ✅ | — | Lista desplegable con los 10 usuarios y su rol. El formulario de compra solo aparece con un comprador. El usuario elegido se pierde al recargar la página (ver Recomendados). |
| Pantalla 2 – Catálogo | ⚠️ | Recomendado | Lista los eventos vigentes como botones, pero muestra solo el nombre. Con la lista vacía no aparece ningún mensaje. |
| Pantalla 3 – Detalle y compra | ⚠️ | Recomendado | Compra, códigos, descuento, cupo y evento cancelado funcionan. Después de cada compra, la modalidad vuelve sola a la primera. La fecha aparece en formato ISO, y un error de cantidad se muestra en inglés técnico. |
| Pantalla 4 – Control de acceso (no obligatoria para equipos de 2) | ⚠️ | Recomendado | Valida, rechaza la revalidación, la entrada inexistente y la de otro evento. Pide el "ID del evento", pero ese dato no aparece en ninguna parte de la interfaz. |
| Pantalla 5 – Consulta de compra (no obligatoria para equipos de 2) | No aplica | — | No está. Tampoco se muestra el número de compra después de comprar. |
| Rol en la interfaz | ✅ | — | El organizador y el usuario sin elegir no ven el formulario de compra. No hay pantallas de gestión ni de reporte, y no hacen falta. |
| Sin frameworks | ✅ | — | HTML, CSS y JS puros. |
| Integración de las dos APIs | ❌ | Obligatorio | Una entrada comprada se valida enseguida y los datos sobreviven al reinicio. Con pedidos simultáneos se pierden datos (ver "Persistencia compartida"). |
| README (APIs y frontend) | ⚠️ | Recomendado | Explica cómo levantar las APIs, correr los tests y abrir el frontend con Live Server. No avisa que `datos.json` no se versiona: al clonar, el catálogo está vacío y hay que crear eventos por Swagger. |
| Tests en verde en las dos lógicas | ✅ | — | Gestión 19/19, validación 9/9. |
| Video de la demo | ❌ | Obligatorio | No hay video en el repositorio ni link en el README. |
| Estructura de carpetas | ✅ | — | `ApiGestion`, `ApiValidacion` y `Frontend` en la raíz, más `Datos`. |
| Google Maps (promoción) | No aplica | — | Nadie en el equipo está en condición de promoción. |
| Cancelación de entradas (promoción) | No aplica | — | Nadie en el equipo está en condición de promoción. |

---

## Pantalla por pantalla

### Pantalla 1 – Elegir usuario

Probé el selector con organizador, con comprador y con "Elegir usuario" (sin usuario). El formulario de compra solo aparece con un comprador y un evento abierto. Si cambio a un organizador con el detalle abierto, se oculta. Eso está bien.

- **[Recomendado]** El usuario elegido no se guarda. Si recargan la página, el selector vuelve a "Elegir usuario" y hay que elegirlo otra vez. En la unidad 7 vimos `localStorage` justamente para recordar este tipo de dato entre recargas. Guárdenlo al cambiar el selector y vuelvan a seleccionarlo al cargar.
- **[Recomendado]** No se muestra en ningún lado con quién se está operando ni qué puede hacer: solo está el valor del desplegable. Un texto del tipo "Operando como Sofía Gómez (Comprador)" ayuda mucho en la demo.

### Pantalla 2 – Catálogo

Aparecen los eventos vigentes como botones con el nombre. Los cancelados y los que ya pasaron no se listan, porque los filtra la API. Probé también con la API caída: aparece "No se pudo conectar con la API de gestión. Comprobá que esté ejecutándose". Con un error 500 aparece "No se pudieron cargar los datos". Las dos cosas están bien.

- **[Recomendado]** Con el catálogo vacío (que es lo que se ve apenas se clona el repositorio) queda el título "Eventos disponibles" sin nada abajo. Muestren un mensaje del tipo "No hay eventos disponibles".
- **[Recomendado]** El catálogo muestra solo el nombre. Para elegir un evento hace falta ver al menos la fecha y el lugar. Pueden armar una tarjeta por evento en lugar de un botón suelto.
- **[Recomendado]** Si fallan los usuarios o los eventos, no se carga ninguno de los dos, porque los dos `fetch` están en el mismo bloque. Conviene separarlos: si la lista de eventos falla, igual se debería poder elegir el usuario.
- **[Recomendado]** El catálogo se carga una sola vez. Si un evento se cancela con la página abierta, sigue apareciendo hasta que se recarga. Por ejemplo, pueden refrescar la lista después de un error de compra.

### Pantalla 3 – Detalle y compra

Lo que probé:

- **Compra de 3 entradas generales** (con Sofía Gómez): "Compra exitosa - Total: $3000 - Entradas:" con 3 códigos alfanuméricos de 6 caracteres. El cupo bajó de 10 a 7 en el detalle.
- **Compra de 5 entradas:** total $4500, o sea, el 10% de descuento está aplicado.
- **Superar el cupo** (3 entradas con 2 disponibles): "No queda cupo suficiente."
- **Cantidad 0 o campo vacío:** "La cantidad debe ser mayor a cero."
- **Evento cancelado:** abrí un evento, lo cancelé desde Swagger con la página abierta e intenté comprar. Apareció "No se puede comprar para un evento cancelado o cuya fecha ya pasó." Está bien.
- **Organizador y usuario sin elegir:** no ven el botón Comprar.

Lo que hay que corregir:

- **[Recomendado]** **La modalidad elegida se pierde después de cada compra.** Al terminar una compra, el detalle se vuelve a dibujar para actualizar el cupo, y el desplegable de modalidades vuelve a la primera opción.
  - Lo que pasó: compré una Platea, después cambié la cantidad y volví a tocar "Comprar". La segunda compra salió como Campo, sin que yo cambiara nada. También compré todas las VIP y la siguiente compra se hizo sobre General.
  - Lo que tiene que pasar: después de comprar tiene que quedar seleccionada la misma modalidad. También pueden pedir que se elija de nuevo, pero nunca se tiene que cambiar sola.
- **[Recomendado]** La fecha se muestra tal como la manda la API ("2026-10-24T23:00:00"). Muéstrenla en un formato legible, por ejemplo "24/10/2026 23:00".
- **[Recomendado]** Una modalidad sin cupo sigue apareciendo igual que las demás, con "Cupo: 0". Márquenla como "Agotado", y deshabiliten esa opción o el botón cuando no queda cupo.
- **[Recomendado]** No se ven los beneficios de cada modalidad. La diferencia entre modalidades es justamente lo que da algo más ("Consumición", "Asiento"), y el comprador lo tiene que poder ver antes de elegir.
- **[Recomendado]** El descuento se aplica bien, pero el comprador solo ve el total. Conviene mostrar el subtotal y el descuento aplicado, o al menos avisar antes de comprar que desde 5 entradas hay 10% de descuento.
- **[Recomendado]** Con una cantidad decimal (2,5) aparece "The pedido field is required. The JSON value could not be converted to System.Int32...". Validen en el frontend que la cantidad sea un entero mayor a cero antes de mandar el pedido, y no muestren mensajes internos de la API.
- **[Recomendado]** El resultado de la compra no dice el número de compra. Aunque no tengan la pantalla 5, ese número es lo que identifica la compra (`GET /api/compras/{id}`). Muéstrenlo junto con los códigos, y presenten los códigos como lista y no como texto corrido.
- **[Recomendado]** Mientras se procesa la compra, el botón sigue habilitado. Deshabilítenlo hasta que llegue la respuesta. Esto además evita el doble envío que se explica en "Persistencia compartida".

### Pantalla 4 – Control de acceso (no obligatoria para equipos de 2)

La sección "Validar entrada" está en la misma página. Pide el ID del evento y el código. Lo que probé:

- Código recién comprado, escrito en minúsculas: "Entrada válida. Ingreso permitido." Está bien que lo normalicen en el frontend y en la API.
- El mismo código otra vez: "La entrada ya fue usada."
- Código inexistente: "La entrada no existe."
- Una entrada del evento 1 validada con el evento 2: "La entrada es de otro evento."
- Sin evento: "Debe indicar el código y el evento."

El circuito con las dos APIs funciona. Lo que hay que mejorar:

- **[Recomendado]** **El "ID del evento" no aparece en ningún lado de la interfaz.** Ni el catálogo ni el detalle muestran el id, así que quien está en la puerta no tiene forma de saber qué número escribir. Reemplacen el campo numérico por un desplegable con los eventos (nombre y fecha), cargado desde la API de gestión.
- **[Recomendado]** El resultado es un texto suelto, igual para "pasa" y para "no pasa". Diferéncienlo con color o con un ícono (verde para "puede pasar", rojo para el rechazo con el motivo) y muestren también la modalidad y la hora de ingreso que ya devuelve la API.
- **[Recomendado]** La validación solo funciona el día del evento (regla que agregaron ustedes en `ServicioValidacion`). Para la demo y el coloquio, entonces, necesitan un evento con fecha de hoy y hora posterior a la actual, porque para comprar la fecha tiene que ser futura. Tengan eso preparado y tengan claro qué pasa con un evento que cruza la medianoche, como ya se marcó en la 1ra entrega.

### Pantalla 5 – Consulta de compra (no obligatoria para equipos de 2)

No está, y no se les pide.

- **[Recomendado]** Si les queda tiempo, es la pantalla que mejor muestra que cada entrada se valida por separado: con el número de compra se lista cada código como "usada" o "sin usar". El endpoint `GET /api/compras/{id}` ya devuelve todo eso.

---

## Persistencia compartida (integración de las dos APIs)

Lo que funciona:
- Compré desde el frontend y validé una de esas entradas desde la sección de validación, sin reiniciar nada.
- Reinicié la API de gestión: los eventos, las compras y el estado "usada" seguían ahí.

Lo que hay que corregir:

- **[Obligatorio para el coloquio]** **Se pierden datos cuando llegan dos pedidos al mismo tiempo.**
  - **Qué cambió:** en la 1ra entrega, cada pedido tomaba un bloqueo exclusivo sobre `Datos/datos.lock` mientras leía, modificaba y guardaba `datos.json`. En el commit `fa430c7` pasaron `Datos` a inyección de dependencias como servicio *scoped*. Ahora cada pedido lee el archivo al principio y lo pisa al final, y el bloqueo se eliminó de `ArchivoDatos`.
  - **Lo que pasó con un doble clic:** hice doble clic en "Comprar" desde el frontend. La segunda compra respondió error 500, y en la pantalla del comprador apareció todo el detalle técnico del error ("System.IO.IOException: The process cannot access the file because it is being used by another process..."), con rutas del disco.
  - **Lo que pasó con pedidos simultáneos:** mandé 20 compras a la vez a gestión y, en simultáneo, 4 validaciones de entradas distintas a validación.
    - Diez compras dieron error 500.
    - Diez respondieron "201 Created" con sus códigos, pero había ids de compra repetidos (tres compras con el id 38 y dos con el 39) y solo 6 quedaron guardadas en el archivo. A cuatro compradores se les confirmaron códigos de entradas que no existen.
    - Las 4 validaciones respondieron "Entrada válida. Ingreso permitido.", pero solo una quedó registrada como usada. Después volví a validar una de las otras y me dejó pasar por segunda vez.
  - **Por qué es obligatorio:** rompe dos reglas de la sección 5. Una entrada se puede usar más de una vez, y se venden entradas que después no existen. Además, la sección 6 pide que las dos APIs compartan los mismos datos de forma consistente.
  - **Qué tiene que pasar:** solo un pedido a la vez, sumando las dos APIs, puede estar en la secuencia "leer, modificar, guardar" de `datos.json`. Pueden recuperar el mecanismo de bloqueo que ya tenían, ahora integrado con la inyección de dependencias. Tienen que poder explicar cómo funciona.
  - **Los errores inesperados:** los controllers solo capturan las excepciones propias y la de permisos. Cualquier otro error (por ejemplo, el de acceso al archivo) sale como 500 con la traza completa, y el frontend la muestra tal cual. La API tiene que devolver un mensaje entendible y el frontend no tiene que mostrar detalles internos.

---

## Rol en la interfaz

- El formulario de compra solo se muestra con un comprador elegido y un evento abierto. El organizador y el usuario sin elegir no lo ven. Además, la API rechaza la compra con 403 si el DNI no es de un comprador.
- No hay pantallas de gestión ni de reporte, así que un comprador no tiene ninguna opción de organizador a la vista, ni siquiera por URL.
- La sección de validación está visible para todos, incluso sin usuario elegido. El enunciado no la restringe por rol, así que es una decisión válida, pero tienen que poder justificarla en el coloquio.

---

## Pendientes de la 1ra entrega

| Punto de `Observaciones.md` | Estado | Comentario |
|---|---|---|
| Carpeta del frontend en la raíz | ✅ Resuelto | `Frontend/`. |
| Borrar `Class1.cs` y el `.http` con `/weatherforecast` | ⚠️ Parcial | Se borraron los dos `Class1.cs` y `Gestion.Api.http`. Queda `Validacion.Api.http` apuntando a `/weatherforecast`. **[Recomendado]** |
| Referencia duplicada en `Validacion.Logica.csproj` | ✅ Resuelto | |
| README: aclarar que hace falta .NET 8 | ✅ Resuelto | |
| Códigos HTTP: 404 si no existe, 400 con body vacío | ✅ Resuelto | Lo verifiqué en editar, cancelar, agregar modalidad y comprar. |
| Errores como objeto con mensaje y no como texto plano | ❌ Pendiente | Siguen volviendo como texto plano, mezclados con los errores de validación automáticos de ASP.NET (que son JSON). Por eso `leerError` del frontend tiene que adivinar el formato. **[Recomendado]** |
| Validación sin evento o con código vacío | ✅ Resuelto | "Debe indicar el código y el evento." |
| Reporte con eventos cancelados | ❌ Pendiente | El reporte sigue sumando las ventas de los eventos cancelados sin indicar que están cancelados. **[Recomendado]** |
| DTOs de request y response | ✅ Resuelto | `PedidoEvento`, `PedidoModalidad`, `PedidoCompra` y `Respuestas.cs`. Probé mandar `id`, `cancelado` y `cupoDisponible` al crear un evento y la API los ignora. |
| Códigos de entrada predecibles | ✅ Resuelto | Ahora son aleatorios y se controlan contra los ya emitidos. Ojo: con la pérdida de datos por concurrencia, ese control tampoco está garantizado (ver "Persistencia compartida"). |
| Normalizar mayúsculas en la validación | ✅ Resuelto | |
| El organizador no ve eventos cancelados ni pasados | ❌ Pendiente | No hace falta para estas pantallas, pero tienen que poder explicar cómo los consultaría el organizador. **[Recomendado]** |
| Repositorios por entidad (unidad 5 y 6) | ❌ Pendiente | Sigue un único `ArchivoDatos` con todo el estado en un archivo, y el controller sigue siendo quien guarda. **[Recomendado]** |
| Explicar el bloqueo del archivo | ❌ Empeoró | El bloqueo se eliminó y eso provoca la pérdida de datos. Ver el punto obligatorio de "Persistencia compartida". |
| Rol como enum | ✅ Resuelto | `RolUsuario`. |
| Namespace de `Usuario` | ✅ Resuelto | |
| Tests: excepción concreta, borde del descuento, códigos distintos, nombres, AAA | ⚠️ Parcial | Hay tests de 4 y 5 entradas y de códigos distintos, los archivos están renombrados y se verifican las excepciones propias. El permiso se sigue verificando con `UnauthorizedAccessException` del framework, y el formato Arrange, Act y Assert está solo en algunos tests. **[Recomendado]** |
| Excepciones propias | ⚠️ Parcial | Están `ReglaNegocioException` y `RecursoNoEncontradoException`. El permiso sigue usando `UnauthorizedAccessException`, que es una excepción del sistema para accesos a archivos y no una regla del dominio. **[Recomendado]** |
| Inyección de dependencias | ✅ Resuelto | Los servicios se registran en `Program.cs` y se reciben en el constructor. Pero se perdió el bloqueo (ver arriba). |
| `Validacion.Logica` acoplada a `Gestion.Logica` | ❌ Pendiente | Sigue igual. Tienen que poder justificarlo en el coloquio. **[Recomendado]** |
| Entidades sin comportamiento (descuento, cupo, marcar usada) | ❌ Pendiente | Todo sigue en los servicios con propiedades públicas modificables. En el coloquio les voy a preguntar dónde vive el descuento y por qué. **[Recomendado]** |
| Inicialización inline de propiedades | ✅ Resuelto | Pasó a los constructores. |
| DNI con el mismo tipo en `Compra` y `Usuario` | ✅ Resuelto | |
| `ReporteEvento` en `Entidades` | ❌ Pendiente | **[Recomendado]** |
| CORS | ✅ Resuelto | Configurado en las dos APIs. |

---

## Calidad del código del frontend

Lo que está bien:
- Usan `fetch` con `async`/`await`, controlan `response.ok` y tienen `try`/`catch` con mensajes para cuando la API no está.
- Hay una función `leerError` que reúne la lectura de los errores.
- Para casi todo el texto usan `textContent`.
- Las dos URLs de las APIs están como constantes al principio de `script.js`.
- Los eventos se manejan con `addEventListener`, incluyendo la delegación para los botones del catálogo.

Lo que hay que mejorar:

- **[Recomendado]** Las opciones de usuarios, los botones de eventos y las opciones de modalidades se arman concatenando texto con `innerHTML +=`, con datos que vienen de la API (nombre del evento, nombre de la modalidad). Si un organizador carga un evento con etiquetas HTML en el nombre, el navegador las interpreta. Creen los elementos con `document.createElement` y asignen el texto con `textContent`, como vimos en la unidad 7. De paso, `innerHTML +=` dentro de un `for` vuelve a dibujar toda la lista en cada vuelta.
- **[Recomendado]** Toda la lógica está en un único `script.js` con un estado global (`eventoId`). Para lo que hay alcanza. Si agregan la pantalla 5 o mejoran la 4, conviene separar las llamadas a la API de las funciones que dibujan la pantalla.
- **[Recomendado]** El diseño es muy básico. En el celular se ve en una columna y se puede usar, pero no hay ninguna regla responsive propia ni un estilo que diferencie estados (éxito, error, agotado). Con lo visto en la unidad 7 (variables CSS, flexbox o grid, `@media`) pueden mejorarlo bastante.
- **[Recomendado]** En la consola del navegador aparece un 404 al cargar la página, porque falta el `favicon`. No afecta el uso, pero ensucia la consola.

---

## README y video

- **[Obligatorio para el coloquio]** **Falta el video de la demo** (sección 11). Tiene que durar entre 1 y 2 minutos y mostrar:
  1. El alta de un evento (por Swagger alcanza).
  2. Una compra con varias entradas desde el frontend.
  3. La validación exitosa de una de esas entradas.
  4. El intento de volver a validarla, rechazado.

  Súbanlo al repositorio o dejen el link en el README. Tengan en cuenta la regla del día del evento: el evento tiene que ser de hoy y con hora posterior a la de la grabación.
- **[Recomendado]** El README explica cómo levantar las dos APIs, correr los tests y abrir el frontend con Live Server. Tiene título e integrantes. Le faltan dos cosas:
  - Aclarar que `Datos/datos.json` no está versionado. Al clonar, el catálogo arranca vacío y hay que crear los eventos desde Swagger con el DNI de un organizador. Dejen un ejemplo de body para crear un evento con modalidades, o un archivo de datos de ejemplo.
  - Aclarar en qué puerto conviene servir el frontend. La CORS acepta cualquier origen, así que también funciona sin Live Server.

---

## Git y trabajo en equipo en esta etapa

- Ahora cada uno commitea con su propio email: Benjamin con 4 commits y Santiago con otros 4. Esto mejoró respecto de la 1ra entrega. Santiago aparece con dos nombres distintos ("Santiago Pettinati" y "santiUcse"), así que unifiquen el `user.name`.
- Los dos tocaron backend y frontend:
  - Santiago hizo los DTOs de pedido, la primera versión de la inyección de dependencias, la primera versión de `script.js` y los tests nuevos.
  - Benjamin hizo el HTML y el CSS, el cambio en la generación de códigos, las excepciones propias y la versión final del frontend.
- **[Recomendado]** Todo el trabajo de esta etapa entró en poco más de 24 horas, entre el 15/10 a las 18:39 y el 16/10 a las 23:29. El último commit, `fa430c7`, entró media hora antes del cierre y modifica 30 archivos (659 líneas agregadas): DTOs de respuesta, enum de rol, cambio de librería de JSON, eliminación del bloqueo, fechas, tests y casi todo el frontend. Esto es lo contrario de lo que pide la sección 9. Lo que queda hasta el coloquio, háganlo con commits chicos y frecuentes, de a un tema por commit, y en lo posible con ramas y pull requests. En el coloquio cada uno va a tener que explicar también lo que commiteó el otro.

---

## Promoción

No aplica: ningún integrante está en condición de promoción.

---

# Resumen final de correcciones

## Obligatorio para el coloquio

1. **Acceso exclusivo a `datos.json` entre las dos APIs.** Hoy, con pedidos simultáneos (alcanza con un doble clic en "Comprar"):
   - se confirman compras que no se guardan;
   - se repiten ids de compra;
   - se registran validaciones que se pierden, y la entrada puede volver a usarse.

   Tienen que recuperar el bloqueo durante toda la secuencia "leer, modificar, guardar". Además, ningún error inesperado puede llegar al usuario con la traza interna. Ver "Persistencia compartida".
2. **Video de la demo** de 1 a 2 minutos: alta de evento, compra de varias entradas, validación exitosa y revalidación rechazada. Va en el repositorio o como link en el README. Ver "README y video".

## Para mantener la promoción (no obligatorio para el coloquio)

No aplica: ningún integrante está en condición de promoción.

## Recomendado (no obligatorio para el coloquio)

1. Que la modalidad elegida no vuelva sola a la primera después de cada compra (pantalla 3).
2. Mostrar el evento en la puerta con un desplegable de eventos y no con un "ID del evento" que no aparece en la interfaz, y diferenciar visualmente "pasa" de "no pasa" (pantalla 4).
3. Tener preparado para la demo un evento con fecha de hoy, y poder explicar la regla del día del evento y el caso de la medianoche (pantalla 4).
4. Mostrar el número de compra, los códigos como lista y el descuento aplicado. Deshabilitar el botón mientras se procesa la compra (pantalla 3).
5. Marcar las modalidades agotadas, mostrar los beneficios de cada modalidad y formatear la fecha (pantalla 3).
6. Validar la cantidad en el frontend y no mostrar mensajes técnicos de la API (pantalla 3).
7. Catálogo: mensaje para la lista vacía, fecha y lugar de cada evento, carga de usuarios independiente de la de eventos, y refrescar la lista (pantalla 2).
8. Guardar el usuario elegido en `localStorage` y mostrar con quién se está operando (pantalla 1).
9. Devolver los errores de la API como objeto con un mensaje, de forma uniforme.
10. Completar el README: `datos.json` arranca vacío, cómo cargar eventos de ejemplo y cómo servir el frontend.
11. Armar los elementos del DOM con `createElement` y `textContent` en lugar de `innerHTML` con datos de la API.
12. Modelar el permiso con una excepción propia y no con `UnauthorizedAccessException`. Completar el formato Arrange, Act y Assert en los tests.
13. Indicar en el reporte de recaudación qué eventos están cancelados.
14. Pendientes de diseño de la 1ra entrega: repositorios por entidad, comportamiento en las entidades (descuento, cupo, marcar usada), `ReporteEvento` fuera de `Entidades`, acoplamiento de `Validacion.Logica` con `Gestion.Logica`, y un listado de eventos completo para el organizador.
15. Borrar `Validacion.Api.http` (sigue apuntando a `/weatherforecast`) y agregar el favicon.
16. Mejorar el estilo y el comportamiento en pantallas chicas con lo visto en la unidad 7.
17. Trabajar con commits chicos y frecuentes, con un solo `user.name` por persona, hasta el coloquio.
18. Si les queda tiempo, hacer la pantalla 5 (consulta de compra con el estado de cada entrada).
