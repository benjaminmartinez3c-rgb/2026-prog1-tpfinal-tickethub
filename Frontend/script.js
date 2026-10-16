
const API = "http://localhost:5175/api";

let eventoId = 0;

async function cargar() {
    const usuarios = await (await fetch(API + "/usuarios")).json();
    const eventos = await (await fetch(API + "/eventos")).json();

    for (let usuario of usuarios) {
        document.getElementById("usuario").innerHTML += `<option value="${usuario.dni}" data-rol="${usuario.rol}">${usuario.nombre} (${usuario.rol})</option>`;
    }

    for (let evento of eventos) {
        document.getElementById("eventos").innerHTML += `<button value="${evento.id}">${evento.nombre}</button>`;
    }
}

function cambiarUsuario() {
    let selector = document.getElementById("usuario");
    let rol = selector.options[selector.selectedIndex].getAttribute("data-rol");

    document.getElementById("compra").hidden =
        rol !== "Comprador" || eventoId === 0 || document.getElementById("modalidad").options.length === 0;
}

async function verEvento(id) {
    const respuesta = await fetch(API + "/eventos/" + id);
    const evento = await respuesta.json();

    eventoId = id;

    document.getElementById("titulo").textContent = evento.nombre;
    document.getElementById("descripcion").textContent = evento.descripcion;
    document.getElementById("lugar").textContent = evento.lugar + " - " + evento.fecha;

    let selector = document.getElementById("modalidad");
    selector.innerHTML = "";

    for (let modalidad of evento.modalidades) {
        selector.innerHTML += `<option value="${modalidad.id}">${modalidad.nombre} - $${modalidad.precio} - Cupo: ${modalidad.cupoDisponible}</option>`;
    }

    document.getElementById("detalle").hidden = false;
    document.getElementById("resultado").textContent = "";

    cambiarUsuario();
}

async function comprar() {
    const pedido = {
        dni: Number(document.getElementById("usuario").value),
        eventoId: eventoId,
        modalidadId: Number(document.getElementById("modalidad").value),
        cantidad: Number(document.getElementById("cantidad").value)
    };

    const respuesta = await fetch(API + "/compras", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(pedido)
    });

    if (!respuesta.ok) {
        document.getElementById("resultado").textContent = await respuesta.text();
        return;
    }

    const compra = await respuesta.json();
    let codigos = "";

    for (let entrada of compra.entradas) {
        codigos += entrada.codigo + " ";
    }

    await verEvento(eventoId);

    document.getElementById("resultado").textContent = "Compra exitosa - Total: $" + compra.total + " - Entradas: " + codigos;
}

document.getElementById("usuario").addEventListener("change", cambiarUsuario);
document.getElementById("comprar").addEventListener("click", comprar);

document.getElementById("eventos").addEventListener("click", function(evento) {
    if (evento.target.tagName === "BUTTON") verEvento(Number(evento.target.value));
});

cargar();
