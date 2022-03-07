using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Data.DBENTITIES
{
    public class CNHDBE
    {
        public int ID { get; set; }

        public string Numero { get; set; }

        public string RENACH { get; set; }

        public string Espelho { get; set; }

        public DateTime DataEmissao { get; set; }

        public DateTime DataValidade { get; set; }

        public CategoriaCNHDBE Categoria { get; set; }

        public DateTime DataInclusao { get; set; }

        public DateTime DataAlteracao { get; set; }

        public bool Status { get; set; }

        public CNHDBE()
        {
            Categoria = new CategoriaCNHDBE();
        }
    }
}
