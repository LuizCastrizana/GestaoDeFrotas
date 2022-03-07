using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Data.DBENTITIES
{
    public class VeiculoDBE
    {
        public int ID { get; set; }

        public string Placa { get; set; }

        public MarcaVeiculoDBE Marca { get; set; }

        public ModeloVeiculoDBE Modelo { get; set; }

        public TipoVeiculoDBE Tipo { get; set; }

        public IEnumerable<MotoristaDBE> ListaMotoristas { get; set; }

        public DateTime DataInclusao { get; set; }

        public DateTime DataAlteracao { get; set; }

        public bool Status { get; set; }

        public VeiculoDBE()
        {
            Marca = new MarcaVeiculoDBE();
            Modelo = new ModeloVeiculoDBE();
            Tipo = new TipoVeiculoDBE();
            ListaMotoristas = Enumerable.Empty<MotoristaDBE>();
        }
    }
}
