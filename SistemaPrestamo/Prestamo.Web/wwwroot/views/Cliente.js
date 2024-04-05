let tablaData;
let idEditar = 0;
const controlador = "Cliente";
const modal = "mdData";
const preguntaEliminar = "Desea eliminar la cliente";
const confirmaEliminar = "El cliente fue eliminado.";
const confirmaRegistro = "Cliente registrado!";

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
            { title: "", "data": "idCliente", visible: false },
            { title: "Nro. Documento", "data": "nroDocumento" },
            { title: "Nombre", "data": "nombre" },
            { title: "Apellido", "data": "apellido" },
            { title: "Correo", "data": "correo" },
            { title: "Telefono", "data": "telefono" },
            { title: "Fecha Creacion", "data": "fechaCreacion" },
            {
                title: "", "data": "idCliente", width: "100px", render: function (data, type, row) {
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

    idEditar = data.idCliente;
    $("#txtNroDocumento").val(data.nroDocumento);
    $("#txtNombre").val(data.nombre);
    $("#txtApellido").val(data.apellido);
    $("#txtCorreo").val(data.correo);
    $("#txtTelefono").val(data.telefono);
    $(`#${modal}`).modal('show');
})  


$("#btnNuevo").on("click", function () {
    idEditar = 0;
    $("#txtNroDocumento").val("");
    $("#txtNombre").val("");
    $("#txtApellido").val("");
    $("#txtCorreo").val("");
    $("#txtTelefono").val("");
    $(`#${modal}`).modal('show');
})

$("#tbData tbody").on("click", ".btn-eliminar", function () {
    var filaSeleccionada = $(this).closest('tr');
    var data = tablaData.row(filaSeleccionada).data();


    Swal.fire({
        text: `${preguntaEliminar} ${data.nombre} ${data.apellido}?`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Si, continuar",
        cancelButtonText: "No, volver"
    }).then((result) => {
        if (result.isConfirmed) {

            fetch(`/${controlador}/Eliminar?Id=${data.idCliente}`, {
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
        idCliente: idEditar,
        NroDocumento: $("#txtNroDocumento").val().trim(),
        Nombre: $("#txtNombre").val().trim(),
        Apellido: $("#txtApellido").val().trim(),
        Correo: $("#txtCorreo").val().trim(),
        Telefono: $("#txtTelefono").val().trim()
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