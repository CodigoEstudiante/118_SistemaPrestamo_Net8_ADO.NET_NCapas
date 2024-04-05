using Microsoft.Extensions.Options;
using Prestamo.Entidades;
using System.Data.SqlClient;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;


namespace Prestamo.Data
{
    public class PrestamoData
    {
        private readonly ConnectionStrings con;
        public PrestamoData(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        public async Task<string> Crear(Prestamo.Entidades.Prestamo objeto)
        {

            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_crearPrestamo", conexion);
                cmd.Parameters.AddWithValue("@IdCliente", objeto.Cliente.IdCliente);
                cmd.Parameters.AddWithValue("@NroDocumento", objeto.Cliente.NroDocumento);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Cliente.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", objeto.Cliente.Apellido);
                cmd.Parameters.AddWithValue("@Correo", objeto.Cliente.Correo);
                cmd.Parameters.AddWithValue("@Telefono", objeto.Cliente.Telefono);
                cmd.Parameters.AddWithValue("@IdMoneda", objeto.Moneda.IdMoneda);
                cmd.Parameters.AddWithValue("@FechaInicio", objeto.FechaInicioPago);
                cmd.Parameters.AddWithValue("@MontoPrestamo", objeto.MontoPrestamo);
                cmd.Parameters.AddWithValue("@InteresPorcentaje", objeto.InteresPorcentaje);
                cmd.Parameters.AddWithValue("@NroCuotas", objeto.NroCuotas);
                cmd.Parameters.AddWithValue("@FormaDePago", objeto.FormaDePago);
                cmd.Parameters.AddWithValue("@ValorPorCuota", objeto.ValorPorCuota);
                cmd.Parameters.AddWithValue("@ValorInteres", objeto.ValorInteres);
                cmd.Parameters.AddWithValue("@ValorTotal", objeto.ValorTotal);
                cmd.Parameters.Add("@msgError", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    respuesta = Convert.ToString(cmd.Parameters["@msgError"].Value)!;
                }
                catch
                {
                    respuesta = "Error al procesar";
                }
            }
            return respuesta;
        }

        public async Task<List<Prestamo.Entidades.Prestamo>> ObtenerPrestamos(int Id, string NroDocumento)
        {
            List<Prestamo.Entidades.Prestamo> lista = new List<Prestamo.Entidades.Prestamo>();

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_obtenerPrestamos", conexion);
                cmd.Parameters.AddWithValue("@IdPrestamo", Id);
                cmd.Parameters.AddWithValue("@NroDocumento", NroDocumento);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteXmlReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        XDocument doc = XDocument.Load(dr);
                        lista = ((doc.Elements("Prestamos")) != null) ? (from prestamo in doc.Element("Prestamos")!.Elements("Prestamo")
                                                                             select new Prestamo.Entidades.Prestamo()
                                                                             {
                                                                                 IdPrestamo = Convert.ToInt32(prestamo.Element("IdPrestamo")!.Value),
                                                                                 Cliente = new Cliente()
                                                                                 {
                                                                                     IdCliente = Convert.ToInt32(prestamo.Element("IdCliente")!.Value),
                                                                                     NroDocumento = prestamo.Element("NroDocumento")!.Value,
                                                                                     Nombre = prestamo.Element("Nombre")!.Value,
                                                                                     Apellido = prestamo.Element("Apellido")!.Value,
                                                                                     Correo = prestamo.Element("Correo")!.Value,
                                                                                     Telefono = prestamo.Element("Telefono")!.Value
                                                                                 },
                                                                                 Moneda = new Moneda
                                                                                 {
                                                                                     IdMoneda = Convert.ToInt32(prestamo.Element("IdMoneda")!.Value),
                                                                                     Nombre = prestamo.Element("NombreMoneda")!.Value,
                                                                                     Simbolo = prestamo.Element("Simbolo")!.Value
                                                                                 },
                                                                                 FechaInicioPago = prestamo.Element("FechaInicioPago")!.Value,
                                                                                 MontoPrestamo = prestamo.Element("MontoPrestamo")!.Value,
                                                                                 InteresPorcentaje = prestamo.Element("InteresPorcentaje")!.Value,
                                                                                 NroCuotas = Convert.ToInt32(prestamo.Element("NroCuotas")!.Value),
                                                                                 FormaDePago = prestamo.Element("FormaDePago")!.Value,
                                                                                 ValorPorCuota = prestamo.Element("ValorPorCuota")!.Value,
                                                                                 ValorInteres = prestamo.Element("ValorInteres")!.Value,
                                                                                 ValorTotal = prestamo.Element("ValorTotal")!.Value,
                                                                                 Estado = prestamo.Element("Estado")!.Value,
                                                                                 FechaCreacion = prestamo.Element("FechaCreacion")!.Value,
                                                                                 PrestamoDetalle = prestamo.Elements("PrestamoDetalle") != null ? (from detalle in prestamo.Element("PrestamoDetalle")!.Elements("Detalle")
                                                                                                                                            select new PrestamoDetalle()
                                                                                                                                            {
                                                                                                                                                IdPrestamoDetalle = Convert.ToInt32(detalle.Element("IdPrestamoDetalle")!.Value),
                                                                                                                                                FechaPago = detalle.Element("FechaPago")!.Value,
                                                                                                                                                MontoCuota = detalle.Element("MontoCuota")!.Value,
                                                                                                                                                NroCuota = Convert.ToInt32(detalle.Element("NroCuota")!.Value),
                                                                                                                                                Estado = detalle.Element("Estado")!.Value,
                                                                                                                                                FechaPagado = detalle.Element("FechaPagado")!.Value
                                                                                                                                            }).ToList() : new List<PrestamoDetalle>()

                                                                             }).ToList() : new List<Prestamo.Entidades.Prestamo>();

                    }
                }
            }
            return lista;
        }

        public async Task<string> PagarCuotas(int IdPrestamo, string NroCuotasPagadas)
        {

            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_pagarCuotas", conexion);
                cmd.Parameters.AddWithValue("@IdPrestamo", IdPrestamo);
                cmd.Parameters.AddWithValue("@NroCuotasPagadas", NroCuotasPagadas);
                cmd.Parameters.Add("@msgError", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    respuesta = Convert.ToString(cmd.Parameters["@msgError"].Value)!;
                }
                catch
                {
                    respuesta = "Error al procesar";
                }
            }
            return respuesta;
        }

    }
}
