using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Data.DBENTITIES
{
    public class MunicipioDBE
    {
        public int ID { get; set; }

        public string NomeMunicipio { get; set; }
        public EstadoDBE Estado { get; set; }

        public string CodigoIbge { get; set; }

        public DateTime DataInclusao { get; set; }

        public DateTime DataAlteracao { get; set; }

        public bool Status { get; set; }

        public MunicipioDBE()
        {
            Estado = new EstadoDBE();
        }
    }
}
