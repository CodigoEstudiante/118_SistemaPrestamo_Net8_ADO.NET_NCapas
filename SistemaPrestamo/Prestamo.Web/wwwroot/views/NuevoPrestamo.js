let tablaData;
let idEditar = 0;
const controlador = "Prestamo";
const confirmaRegistro = "Prestamo registrado!";

let idCliente = 0;

document.addEventListener("DOMContentLoaded", function (event) {
    $.LoadingOverlay("show");
    fetch(`/Moneda/Lista`, {
        method: "GET",
        headers: { 'Content-Type': 'application/json;charset=utf-8' }
    }).then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        $.LoadingOverlay("hide");
        if (responseJson.data.length > 0) {
            responseJson.data.forEach((item) => {
                $("#cboTipoMoneda").append($("<option>").val(item.idMoneda).text(item.nombre));
            });
        }
    }).catch((error) => {
        $.LoadingOverlay("hide");
        Swal.fire({
            title: "Error!",
            text: "No se pudo eliminar.",
            icon: "warning"
        });
    })
});


$("#btnBuscar").on("click", function () {
    
    if ($("#txtNroDocumento").val() == "") {
        Swal.fire({
            title: "Ups!",
            text: "Debe ingresar un numero de documento.",
            icon: "warning"
        });
        return;
    }

    $("#cardCliente").LoadingOverlay("show");

    fetch(`/${controlador}/ObtenerCliente?NroDocumento=${$("#txtNroDocumento").val()}`, {
        method: "GET",
        headers: { 'Content-Type': 'application/json;charset=utf-8' }
    }).then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        $("#cardCliente").LoadingOverlay("hide");
        if (responseJson.data.idCliente != 0) {

            const cliente = responseJson.data;
            idCliente = cliente.idCliente;
            $("#txtNombre").val(cliente.nombre);
            $("#txtApellido").val(cliente.apellido);
            $("#txtCorreo").val(cliente.correo);
            $("#txtTelefono").val(cliente.telefono);


        } else {
            $("#txtNombre").val('');
            $("#txtApellido").val('');
            $("#txtCorreo").val('');
            $("#txtTelefono").val('');
            Swal.fire({
                title: "No se encontro un cliente registrado",
                text: `Desea registrar manualmente?`,
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Si, continuar",
                cancelButtonText: "No, volver"
            }).then((result) => {
                if (result.isConfirmed) {
                    idCliente = 0;
                    $("#txtNombre").removeAttr('disabled');
                    $("#txtApellido").removeAttr('disabled');
                    $("#txtCorreo").removeAttr('disabled');
                    $("#txtTelefono").removeAttr('disabled');
                }
            });
        }
    }).catch((error) => {
        $("#cardCliente").LoadingOverlay("hide");
        Swal.fire({
            title: "Error!",
            text: "No se pudo eliminar.",
            icon: "warning"
        });
    })
  
})

$("#btnCalcular").on("click", function () {
    const inputsPrestamo = $(".data-prestamo").serializeArray();
    const inputText = inputsPrestamo.find((e) => e.value == "");
    
    
    if (inputText != undefined) {
        Swal.fire({
            title: "Error!",
            text: `Debe completar el campo: ${inputText.name.replaceAll("_"," ")}`,
            icon: "warning"
        });
        return
    }
    const montoPrestamo = parseFloat(inputsPrestamo.find((e) => e.name == "Monto_Prestamo").value);
    const interes = parseFloat(inputsPrestamo.find((e) => e.name == "Interes").value);
    const nroCuotas = parseFloat(inputsPrestamo.find((e) => e.name == "NroCuotas").value);

    const montoInteres = montoPrestamo * (interes / 100);
    const montoTotal = montoPrestamo + montoInteres;
    const montoPorCuota = montoTotal / nroCuotas;

    $("#txtMontoInteres").val(montoInteres.toFixed(2));
    $("#txtMontoPorCuota").val(montoPorCuota.toFixed(2));
    $("#txtMontoTotal").val(montoTotal.toFixed(2));

})

$("#btnRegistrar").on("click", function () {
    const inputs = $(".data-in").serializeArray();
    const inputText = inputs.find((e) => e.value == "");

    if (idCliente == 0) {
        if (inputText != undefined) {
            Swal.fire({
                title: "Error!",
                text: `Debe completar el campo: ${inputText.name.replaceAll("_", " ")}`,
                icon: "warning"
            });
            return
        }
    }
   

    if ($("#txtMontoTotal").val() == "") {
        Swal.fire({
            title: "Error!",
            text: `Debe completar el detalle del prestamo`,
            icon: "warning"
        });
        return
    }


    const objeto = {
        Cliente: {
            IdCliente: idCliente,
            NroDocumento: $("#txtNroDocumento").val(),
            Nombre: $("#txtNombre").val(),
            Apellido: $("#txtApellido").val(),
            Correo: $("#txtCorreo").val(),
            Telefono: $("#txtTelefono").val() 
        },
        Moneda: {
            IdMoneda: $("#cboTipoMoneda").val()
        },
        FechaInicioPago: moment($("#txtFechaInicio").val()).format("DD/MM/YYYY"),
        MontoPrestamo: $("#txtMontoPrestamo").val(),
        InteresPorcentaje: $("#txtInteres").val(),
        NroCuotas: $("#txtNroCuotas").val(),
        FormaDePago: $("#cboFormaPago").val(),
        ValorPorCuota: $("#txtMontoPorCuota").val(),
        ValorInteres: $("#txtMontoInteres").val(),
        ValorTotal: $("#txtMontoTotal").val()
    }

    fetch(`/${controlador}/Crear`, {
        method: "POST",
        headers: { 'Content-Type': 'application/json;charset=utf-8' },
        body: JSON.stringify(objeto)
    }).then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        if (responseJson.data == "") {

            idCliente = 0;
            $(".data-in").val("");
            $(".data-prestamo").val("");
            $("#cboTipoMoneda").val($("#cboTipoMoneda option:first").val());
            $("#cboFormaPago").val($("#cboFormaPago option:first").val());

            Swal.fire({
                title: "Listo!",
                text: confirmaRegistro,
                icon: "success"
            });
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
})




