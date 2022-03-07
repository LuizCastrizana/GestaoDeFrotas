using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Data.DBENTITIES
{
    public class MotoristaDBE
    {
        public int ID { get; set; }

        public string PrimeiroNome { get; set; }

        public string Sobrenome { get; set; }

        public DateTime DataNascimento { get; set; }

        public string CPF { get; set; }

        public string RG { get; set; }

        public CNHDBE CNH { get; set; }

        public IEnumerable<VeiculoDBE> ListaVeiculos { get; set; }

        public EnderecoDBE Endereco { get; set; }

        public DateTime DataInclusao { get; set; }

        public DateTime DataAlteracao { get; set; }

        public bool Status { get; set; }

        public MotoristaDBE()
        {
            Endereco = new EnderecoDBE();
            CNH = new CNHDBE();
            ListaVeiculos = Enumerable.Empty<VeiculoDBE>();
        }
    }
}
