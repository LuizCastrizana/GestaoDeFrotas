using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Data.DBENTITIES
{
    public class EnderecoDBE
    {
        public int ID { get; set; }

        public string Logradouro { get; set; }

        public int? Numero { get; set; }

        public string Complemento { get; set; }

        public string Bairro { get; set; }

        public MunicipioDBE Municipio { get; set; }

        public string Cep { get; set; }

        public DateTime DataInclusao { get; set; }

        public DateTime DataAlteracao { get; set; }

        public bool Status { get; set; }

        public EnderecoDBE()
        {
            Municipio = new MunicipioDBE();
            Complemento = "";
            Bairro = "";
        }
    }
}
