let tablaData;
let idPrestamo = 0;
const controlador = "Prestamo";
const modal = "mdData";
const preguntaEliminar = "Desea eliminar la moneda";
const confirmaEliminar = "La moneda fue eliminada.";
const confirmaRegistro = "Moneda registrada!";

document.addEventListener("DOMContentLoaded", function (event) {

    tablaData = $('#tbData').DataTable({
        responsive: true,
        scrollX: true,
        "ajax": {
            "url": `/${controlador}/ObtenerPrestamos?IdPrestamo=0&NroDocumento=`,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { title: "Nro. Prestamo", "data": "idPrestamo" },
            {
                title: "Cliente", "data": "cliente", render: function (data, type, row) {
                    return `${data.nombre} ${data.apellido}`
                }
            },
            { title: "Monto Prestamo", "data": "montoPrestamo" },
            { title: "Monto Interes", "data": "valorInteres" },
            { title: "Monto Total", "data": "valorTotal" },
            {
                title: "Moneda", "data": "moneda", render: function (data, type, row) {
                    return `${data.nombre}`
                }
            },
            {
                title: "Estado", "data": "estado", render: function (data, type, row) {
                    return data == "Pendiente" ? '<span class="badge bg-danger p-2">Pendiente</span>' : '<span class="badge bg-success p-2">Cancelado</span>'
                }
            },
            {
                title: "", "data": "idPrestamo", width: "120px", render: function (data, type, row) {
                    return `<button class="btn btn-primary me-2 btn-detalle"><i class="fa-solid fa-list-ol"></i> Ver detalle</button>`
                }
            }
        ],
        "order": [0, 'desc'],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

});


$("#tbData tbody").on("click", ".btn-detalle", function () {
    const filaSeleccionada = $(this).closest('tr');
    const prestamo = tablaData.row(filaSeleccionada).data();
    const detalle = prestamo.prestamoDetalle;
    idPrestamo = prestamo.idPrestamo;
    $("#txtIdPrestamo").text(`Nro. Prestamo: ${prestamo.idPrestamo}`)
    $("#txtMontoPrestamo").val(prestamo.montoPrestamo)
    $("#txtInteres").val(prestamo.interesPorcentaje)
    $("#txtNroCuotas").val(prestamo.nroCuotas)
    $("#txtFormaPago").val(prestamo.formaDePago)
    $("#txtTipoMoneda").val(prestamo.moneda.nombre)
    $("#txtMontoTotal").val(prestamo.valorTotal)

    $("#tbDetalle tbody").html("");

    detalle.forEach(function (e) {
        $("#tbDetalle tbody").append(`<tr>
                                   <td>${e.nroCuota}</td>
                                   <td>${e.fechaPago}</td>
                                   <td>${e.montoCuota}</td>
                                   <td>${e.estado}</td>
                               </tr>`);
    })
    


    $(`#${modal}`).modal('show');
})  

$("#btnImprimir").on("click", function () {
    window.open(`/Prestamo/ImprimirPrestamo?IdPrestamo=${idPrestamo}`, "_blank");
})