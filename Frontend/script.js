const API = "http://localhost:5175/api";
const API_VALIDACION = "http://localhost:5012/api/validaciones";

let eventoId = 0;
let comprando = false;

async function leerError(respuesta) {
    if (respuesta.status >= 500)
        return "No se pudo completar la operación. Intentá nuevamente.";
    const texto = await respuesta.text();

    try {
        const error = JSON.parse(texto);

        if (error.errors) {
            let mensaje = "";

            for (let campo in error.errors) {
                mensaje += error.errors[campo].join(" ") + " ";
            }

            return mensaje.trim();
        }

        return error.title || texto;
    } catch {
        return texto;
    }
}

async function cargar() {
    const mensaje = document.getElementById("mensaje");

    try {
        const respuestaUsuarios = await fetch(API + "/usuarios");
        const respuestaEventos = await fetch(API + "/eventos");

        if (!respuestaUsuarios.ok || !respuestaEventos.ok) {
            mensaje.textContent = "No se pudieron cargar los datos.";
            return;
        }

        const usuarios = await respuestaUsuarios.json();
        const eventos = await respuestaEventos.json();

        for (let usuario of usuarios) {
            document.getElementById("usuario").innerHTML +=
                `<option value="${usuario.dni}" data-rol="${usuario.rol}">${usuario.nombre} (${usuario.rol})</option>`;
        }

        for (let evento of eventos) {
            document.getElementById("eventos").innerHTML +=
                `<button value="${evento.id}">${evento.nombre}</button>`;
        }

        mensaje.textContent = "";
    } catch {
        mensaje.textContent =
            "No se pudo conectar con la API de gestión. Comprobá que esté ejecutándose.";
    }
}

function cambiarUsuario() {
    const selector = document.getElementById("usuario");
    const rol = selector.options[selector.selectedIndex]
        .getAttribute("data-rol");

    const modalidades = document.getElementById("modalidad");
    let sinCupo = true;

    if (modalidades.selectedIndex >= 0) {
        const opcion = modalidades.options[modalidades.selectedIndex];
        sinCupo = Number(opcion.getAttribute("data-cupo")) <= 0;
    }

    document.getElementById("compra").hidden =
        rol !== "Comprador" ||
        eventoId === 0 ||
        modalidades.options.length === 0;

    document.getElementById("comprar").disabled =
        comprando || sinCupo;
}

async function verEvento(id, conservarResultado = false) {
    const resultado = document.getElementById("resultado");

    try {
        const respuesta = await fetch(API + "/eventos/" + id);

        if (!respuesta.ok) {
            document.getElementById("mensaje").textContent =
                "No se pudo cargar el evento.";
            return;
        }

        const evento = await respuesta.json();
        eventoId = id;

        document.getElementById("titulo").textContent = evento.nombre;
        document.getElementById("descripcion").textContent = evento.descripcion;
        document.getElementById("lugar").textContent =
            evento.lugar + " - " + evento.fecha;

        const selector = document.getElementById("modalidad");
        const modalidadAnterior = selector.value;

        selector.innerHTML = "";

        for (let modalidad of evento.modalidades) {
            let disponibilidad = "Cupo: " + modalidad.cupoDisponible;

            if (modalidad.cupoDisponible === 0)
                disponibilidad = "Agotado";

            selector.innerHTML +=
                `<option value="${modalidad.id}" data-cupo="${modalidad.cupoDisponible}">${modalidad.nombre} - $${modalidad.precio} - ${disponibilidad}</option>`;
        }

        if (conservarResultado && modalidadAnterior !== "")
            selector.value = modalidadAnterior;

        document.getElementById("detalle").hidden = false;
        document.getElementById("mensaje").textContent = "";

        if (!conservarResultado)
            resultado.textContent = "";

        cambiarUsuario();
    } catch {
        document.getElementById("mensaje").textContent =
            "No se pudo actualizar el evento. Comprobá que la API de gestión esté ejecutándose.";
    }
}

async function comprar() {
    if (comprando)
        return;

    const resultado = document.getElementById("resultado");
    const cantidad = Number(document.getElementById("cantidad").value);

    if (!Number.isInteger(cantidad) || cantidad <= 0) {
        resultado.textContent =
            "Ingresá una cantidad entera mayor a cero.";
        return;
    }

    const pedido = {
        dni: Number(document.getElementById("usuario").value),
        eventoId: eventoId,
        modalidadId: Number(document.getElementById("modalidad").value),
        cantidad: cantidad
    };

    comprando = true;
    cambiarUsuario();

    try {
        const respuesta = await fetch(API + "/compras", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(pedido)
        });

        if (!respuesta.ok) {
            resultado.textContent = await leerError(respuesta);
            return;
        }

        const compra = await respuesta.json();
        let codigos = "";

        for (let entrada of compra.entradas)
            codigos += entrada.codigo + " ";

        resultado.textContent =
            "Compra exitosa - Total: $" + compra.total + " - Entradas: " + codigos;

        await verEvento(pedido.eventoId, true);
    } catch {
        resultado.textContent =
            "No se recibió la confirmación de compra. Revisá la conexión antes de volver a intentarlo.";
    } finally {
        comprando = false;
        cambiarUsuario();
    }
}

async function validarEntrada() {
    const resultado = document.getElementById("resultadoValidar");

    const pedido = {
        codigo: document.getElementById("codigoValidar")
            .value.trim().toUpperCase(),
        eventoId: Number(document.getElementById("eventoValidar").value)
    };

    try {
        const respuesta = await fetch(API_VALIDACION, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(pedido)
        });

        if (respuesta.ok) {
            resultado.textContent = "Entrada válida. Ingreso permitido.";
        } else {
            resultado.textContent = await leerError(respuesta);
        }
    } catch {
        resultado.textContent = "No se recibió la confirmación del ingreso. Comprobá que la API de validación esté ejecutándose.";
    }
}

document.getElementById("usuario").addEventListener("change", cambiarUsuario);

document.getElementById("modalidad").addEventListener("change", cambiarUsuario);
document.getElementById("comprar").addEventListener("click", comprar);
document.getElementById("validar").addEventListener("click", validarEntrada);

document.getElementById("eventos").addEventListener("click", function(evento) {
    if (evento.target.tagName === "BUTTON") verEvento(Number(evento.target.value));
});

cargar();
