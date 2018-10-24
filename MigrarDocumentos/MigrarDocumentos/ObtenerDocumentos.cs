using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrarDocumentos
{
    public class ObtenerDocumentos
    {
        public List<E_DOCUMENTO> ListaDocumentos(string pNameServer, string pNameCatalog, string pId, string pPassword, string pID, string pClTipoDocumento)
        {
            List<E_DOCUMENTO> vLstDocumentos = new List<E_DOCUMENTO>();

            using (SqlConnection cn = new SqlConnection("Data Source=" + pNameServer + ";initial catalog=" + pNameCatalog + ";persist security info=True;user id=" + pId + ";password=" + pPassword + ";"))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("ADM.SPE_OBTIENE_DOCUMENTOS", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@PIN_ID", SqlDbType.NVarChar).Value = pID;
                cmd.Parameters.Add("@PIN_CL_TIPO_DOCUMENTO", SqlDbType.NVarChar).Value = pClTipoDocumento;

                SqlDataReader dr = cmd.ExecuteReader();

                //if (dr.Read())
                //{
                foreach (IDataRecord record in GetFromReader(dr))
                {
                    vLstDocumentos.Add(new E_DOCUMENTO
                    {
                        ID_DOCUMENTO = int.Parse(record["ID_DOCUMENTO"].ToString()),
                        CL_TIPO_DOCUMENTO = record["CL_TIPO_DOCUMENTO"].ToString(),
                        CL_ORIGEN_DOCUMENTO = record["CL_ORIGEN_DOCUMENTO"].ToString(),
                        NUMERO_DOCUMENTO_ORIGEN = int.Parse(record["NUMERO_DOCUMENTO_ORIGEN"].ToString()),
                        NB_DOCUMENTO = record["NB_DOCUMENTO"].ToString(),
                        DS_DOCUMENTO = record["DS_DOCUMENTO"].ToString(),
                        DS_NOTAS = record["DS_NOTAS"].ToString()
                    });
                    //}
                }

                cn.Close();
            }

            return vLstDocumentos;
        }

        public void ConvertirDocumentos(List<E_DOCUMENTO> pLstDocumentos, string pNameServer, string pNameCatalog, string pId, string pPassword)
        {
            using (SqlConnection cn = new SqlConnection("Data Source=" + pNameServer + ";initial catalog=" + pNameCatalog + ";persist security info=True;user id=" + pId + ";password=" + pPassword + ";"))
            {
                cn.Open();

                foreach (E_DOCUMENTO item in pLstDocumentos)
                {
                    item.FI_DOCUMENTO = ObtenerArreglo(item.NB_DOCUMENTO, item.CL_ORIGEN_DOCUMENTO);

                    if (item.FI_DOCUMENTO != null)
                    {

                        SqlCommand cmd = new SqlCommand("ADM.SPE_INSERTA_C_DOCUMENTO_MIGRACION", cn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@PIN_NB_DOCUMENTO", SqlDbType.NVarChar).Value = GenerarNombre(item.DS_DOCUMENTO, item.NB_DOCUMENTO);
                        cmd.Parameters.Add("@PIN_CL_DOCUMENTO", SqlDbType.NVarChar).Value = item.CL_ORIGEN_DOCUMENTO;
                        cmd.Parameters.Add("@PIN_CL_TIPO_DOCUMENTO", SqlDbType.NVarChar).Value = "OTRO";
                        cmd.Parameters.Add("@PIN_ID_CANDIDATO", SqlDbType.NVarChar).Value = item.CL_ORIGEN_DOCUMENTO == "SOLICITUD" ? item.CL_TIPO_DOCUMENTO : "";
                        cmd.Parameters.Add("@PIN_ID_EMPLEADO", SqlDbType.NVarChar).Value = item.CL_ORIGEN_DOCUMENTO == "EMPLEADO" ? item.CL_TIPO_DOCUMENTO : "";
                        cmd.Parameters.Add("@PIN_ID_PROCEDENCIA", SqlDbType.NVarChar).Value = item.CL_ORIGEN_DOCUMENTO == "INSTRUCTOR" ? item.CL_TIPO_DOCUMENTO : item.CL_ORIGEN_DOCUMENTO == "CURSO" ? item.CL_TIPO_DOCUMENTO : item.CL_ORIGEN_DOCUMENTO == "PUESTO" ? item.CL_TIPO_DOCUMENTO : "";
                        cmd.Parameters.Add("@PIN_CL_PROCEDENCIA", SqlDbType.NVarChar).Value = item.CL_ORIGEN_DOCUMENTO == "INSTRUCTOR" ? item.CL_ORIGEN_DOCUMENTO : item.CL_ORIGEN_DOCUMENTO == "CURSO" ? item.CL_ORIGEN_DOCUMENTO : item.CL_ORIGEN_DOCUMENTO == "PUESTO" ? item.CL_ORIGEN_DOCUMENTO : "";
                        cmd.Parameters.Add("@PIN_FI_ARCHIVO", SqlDbType.VarBinary).Value = item.FI_DOCUMENTO;
                        cmd.Parameters.Add("@PIN_DS_NOTAS", SqlDbType.NVarChar).Value = item.DS_NOTAS == null ? "" : item.DS_NOTAS;

                        cmd.ExecuteNonQuery();
                   }

                }

                cn.Close();
            }
        }

        IEnumerable<IDataRecord> GetFromReader(IDataReader reader)
        {
            while (reader.Read()) yield return reader;
        }

        public byte[] ObtenerArreglo(string pNbDocuemnto, string pClOrigen)
        {
            byte[] vFiArchivo = null;

            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fullPath = path + "\\Docuqdi\\" + pNbDocuemnto;

                if (File.Exists(fullPath))
            {


                vFiArchivo = System.IO.File.ReadAllBytes(fullPath);
            }

            return vFiArchivo;
        }

        public string GenerarNombre(string pNombre, string pFormato)
        {
            string vNombreDocumento = "";

            String[] vCadenaNombre = pFormato.Split('.');
            if (vCadenaNombre.Length > 0)
                vNombreDocumento = pNombre + "." + vCadenaNombre[1].ToString();


            return vNombreDocumento;
        }

    }
}
