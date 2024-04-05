
document.addEventListener("DOMContentLoaded", function (event) {
    $.LoadingOverlay("show");
    fetch(`/Home/ObtenerResumen`, {
        method: "GET",
        headers: { 'Content-Type': 'application/json;charset=utf-8' }
    }).then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        $.LoadingOverlay("hide");
        console.log(responseJson.data)
        if (responseJson.data != undefined) {
            const r = responseJson.data;
            $("#spInteresAcumulado").text(r.interesAcumulado);
            $("#spPrestamosCancelados").text(r.prestamosCancelados);
            $("#spPrestamosPendientes").text(r.prestamosPendientes);
            $("#spTotalClientes").text(r.totalClientes);
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

