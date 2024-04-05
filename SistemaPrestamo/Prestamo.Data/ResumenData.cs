using Microsoft.Extensions.Options;
using Prestamo.Entidades;
using System.Data.SqlClient;
using System.Data;
namespace Prestamo.Data
{
    public class ResumenData
    {
        private readonly ConnectionStrings con;
        public ResumenData(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        public async Task<Resumen> Obtener()
        {
            Resumen objeto = new Resumen();

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_obtenerResumen", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        objeto = new Resumen()
                        {
                            TotalClientes = dr["TotalClientes"].ToString()!,
                            PrestamosPendientes = dr["PrestamosPendientes"].ToString()!,
                            PrestamosCancelados = dr["PrestamosCancelados"].ToString()!,
                            InteresAcumulado = dr["InteresAcumulado"].ToString()!,
                        };
                    }
                }
            }
            return objeto;
        }


    }
}
