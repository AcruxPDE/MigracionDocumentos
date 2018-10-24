using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrarDocumentos
{
    class MigrarDocumentosPrincipal
    {
        static void Main(string[] args)
        {
            try
            {
                ObtenerDocumentos objDocumentos = new ObtenerDocumentos();
                List<E_DOCUMENTO> vLstDocumentos = objDocumentos.ListaDocumentos("cientes5.cdpvkxmilodr.us-west-2.rds.amazonaws.com,1433", "bruken", "admin", "BeUFAwD4q5", "",""); //Los ultimos dos parametros no se utilizan en el SP
                if (vLstDocumentos.Count > 0)
                    objDocumentos.ConvertirDocumentos(vLstDocumentos, "cientes5.cdpvkxmilodr.us-west-2.rds.amazonaws.com,1433", "QDI50_ca", "admin", "BeUFAwD4q5");
                else
                    Console.WriteLine("No existen documentos que migrar.");
            }
            catch(Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }

        }
    }
}
