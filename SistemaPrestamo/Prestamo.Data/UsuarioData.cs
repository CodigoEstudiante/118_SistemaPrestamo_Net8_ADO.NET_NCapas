using Microsoft.Extensions.Options;
using Prestamo.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prestamo.Data
{
    public class UsuarioData
    {
        private readonly ConnectionStrings con;
        public UsuarioData(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        public async Task<Usuario> Obtener(string correo,string clave)
        {
            Usuario objeto = null!;

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_obtenerUsuario", conexion);
                cmd.Parameters.AddWithValue("@Correo", correo);
                cmd.Parameters.AddWithValue("@Clave", clave);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        objeto = new Usuario()
                        {
                            IdUsuario = Convert.ToInt32(dr["IdUsuario"].ToString()!),
                            NombreCompleto = dr["NombreCompleto"].ToString()!,
                            Correo = dr["Correo"].ToString()!
                        };
                    }
                }
            }
            return objeto;
        }
    }
}
