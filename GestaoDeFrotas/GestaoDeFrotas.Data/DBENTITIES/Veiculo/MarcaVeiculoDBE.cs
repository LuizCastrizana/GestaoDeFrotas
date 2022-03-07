using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Data.DBENTITIES
{
    public class MarcaVeiculoDBE
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public bool Status { get; set; }
    }
}
