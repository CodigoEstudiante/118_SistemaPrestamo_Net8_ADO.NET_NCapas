using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using System.Data;
using Prestamo.Entidades;

namespace Prestamo.Data
{
    public class MonedaData
    {
        private readonly ConnectionStrings con;
        public MonedaData(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        public async Task<List<Moneda>> Lista()
        {
            List<Moneda> lista = new List<Moneda>();

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_listaMoneda", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Moneda()
                        {
                            IdMoneda = Convert.ToInt32(dr["IdMoneda"]),
                            Nombre = dr["Nombre"].ToString()!,
                            Simbolo = dr["Simbolo"].ToString()!,
                            FechaCreacion = dr["FechaCreacion"].ToString()!
                        });
                    }
                }
            }
            return lista;
        }

        public async Task<string> Crear(Moneda objeto)
        {

            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_crearMoneda", conexion);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@Simbolo", objeto.Simbolo);
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

        public async Task<string> Editar(Moneda objeto)
        {

            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_editarMoneda", conexion);
                cmd.Parameters.AddWithValue("@IdMoneda", objeto.IdMoneda);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@Simbolo", objeto.Simbolo);
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

        public async Task<string> Eliminar(int Id)
        {

            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_eliminarMoneda", conexion);
                cmd.Parameters.AddWithValue("@IdMoneda", Id);
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
