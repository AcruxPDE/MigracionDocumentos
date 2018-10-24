using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrarDocumentos
{
    public class E_DOCUMENTO
    {
        public int? ID_DOCUMENTO { get; set; }
        public string CL_TIPO_DOCUMENTO { get; set; }
        public string CL_ORIGEN_DOCUMENTO { get; set; }
        public int NUMERO_DOCUMENTO_ORIGEN { get; set; }
        public string NB_DOCUMENTO { get; set; }
        public string DS_DOCUMENTO { get; set; }
        public string DS_NOTAS { get; set; }
        public DateTime FE_INGRESO_DOCUMENTO { get; set; }
        public byte[] FI_DOCUMENTO { get; set; }
    }
}
