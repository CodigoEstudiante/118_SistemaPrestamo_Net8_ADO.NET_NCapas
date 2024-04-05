let tablaData;
let idEditar = 0;
const controlador = "Moneda";
const modal = "mdData";
const preguntaEliminar = "Desea eliminar la moneda";
const confirmaEliminar = "La moneda fue eliminada.";
const confirmaRegistro = "Moneda registrada!";

document.addEventListener("DOMContentLoaded", function (event) {

    tablaData = $('#tbData').DataTable({
        responsive: true,
        scrollX: true,
        "ajax": {
            "url": `/${controlador}/Lista`,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { title: "", "data": "idMoneda", visible: false },
            { title: "Nombre", "data": "nombre" },
            { title: "Simbolo", "data": "simbolo" },
            { title: "Fecha Creacion", "data": "fechaCreacion" },
            {
                title: "", "data": "idMoneda", width: "100px", render: function (data, type, row) {
                    return `<button class="btn btn-primary me-2 btn-editar"><i class="fa-solid fa-pen"></i></button>` +
                        `<button class="btn btn-danger btn-eliminar"><i class="fa-solid fa-trash"></i></button>`
                }
            }
        ],
        "order": [0, 'desc'],
        fixedColumns: {
            start: 0,
            end: 1
        },
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

});


$("#tbData tbody").on("click", ".btn-editar", function () {
    var filaSeleccionada = $(this).closest('tr');
    var data = tablaData.row(filaSeleccionada).data();

    idEditar = data.idMoneda;
    $("#txtNombre").val(data.nombre);
    $("#txtSimbolo").val(data.simbolo);
    $(`#${modal}`).modal('show');
})  


$("#btnNuevo").on("click", function () {
    idEditar = 0;
    $("#txtNombre").val("");
    $("#txtSimbolo").val("");
    $(`#${modal}`).modal('show');
})

$("#tbData tbody").on("click", ".btn-eliminar", function () {
    var filaSeleccionada = $(this).closest('tr');
    var data = tablaData.row(filaSeleccionada).data();


    Swal.fire({
        text: `${preguntaEliminar} ${data.nombre}?`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Si, continuar",
        cancelButtonText: "No, volver"
    }).then((result) => {
        if (result.isConfirmed) {

            fetch(`/${controlador}/Eliminar?Id=${data.idMoneda}`, {
                method: "DELETE",
                headers: { 'Content-Type': 'application/json;charset=utf-8' }
            }).then(response => {
                return response.ok ? response.json() : Promise.reject(response);
            }).then(responseJson => {
                if (responseJson.data == "") {
                    Swal.fire({
                        title: "Listo!",
                        text: confirmaEliminar,
                        icon: "success"
                    });
                    tablaData.ajax.reload();
                } else {
                    Swal.fire({
                        title: "Error!",
                        text: "No se pudo eliminar.",
                        icon: "warning"
                    });
                }
            }).catch((error) => {
                Swal.fire({
                    title: "Error!",
                    text: "No se pudo eliminar.",
                    icon: "warning"
                });
            })
        }
    });
})



$("#btnGuardar").on("click", function () {
    const inputs = $(".data-in").serializeArray();
    const inputText = inputs.find((e) => e.value == "");

    if (inputText != undefined) {
        Swal.fire({
            title: "Error!",
            text: `Debe completar el campo: ${inputText.name}`,
            icon: "warning"
        });
        return
    }

    let objeto = {
        IdMoneda: idEditar,
        Nombre: $("#txtNombre").val().trim(),
        Simbolo: $("#txtSimbolo").val().trim()
    }

    if (idEditar != 0) {

        fetch(`/${controlador}/Editar`, {
            method: "PUT",
            headers: { 'Content-Type': 'application/json;charset=utf-8' },
            body: JSON.stringify(objeto)
        }).then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        }).then(responseJson => {
            if (responseJson.data == "") {
                idEditar = 0;
                Swal.fire({
                    text: "Se guardaron los cambios!",
                    icon: "success"
                });
                $(`#${modal}`).modal('hide');
                tablaData.ajax.reload();
            } else {
                Swal.fire({
                    title: "Error!",
                    text: responseJson.data,
                    icon: "warning"
                });
            }
        }).catch((error) => {
            Swal.fire({
                title: "Error!",
                text: "No se pudo editar.",
                icon: "warning"
            });
        })
    } else {
        fetch(`/${controlador}/Crear`, {
            method: "POST",
            headers: { 'Content-Type': 'application/json;charset=utf-8' },
            body: JSON.stringify(objeto)
        }).then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        }).then(responseJson => {
            if (responseJson.data == "") {
                Swal.fire({
                    text: confirmaRegistro,
                    icon: "success"
                });
                $(`#${modal}`).modal('hide');
                tablaData.ajax.reload();
            } else {
                Swal.fire({
                    title: "Error!",
                    text: responseJson.data,
                    icon: "warning"
                });
            }
        }).catch((error) => {
            Swal.fire({
                title: "Error!",
                text: "No se pudo registrar.",
                icon: "warning"
            });
        })
    }
});