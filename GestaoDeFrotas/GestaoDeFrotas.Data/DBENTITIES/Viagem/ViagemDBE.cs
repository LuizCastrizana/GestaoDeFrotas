using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Data.DBENTITIES
{
    public class ViagemDBE
    {
        public int ID { get; set; }

        public string Codigo { get; set; }

        public MotoristaDBE MotoristaViagem { get; set; }

        public VeiculoDBE VeiculoViagem { get; set; }

        public DateTime Inicio { get; set; }

        public DateTime Fim { get; set; }

        public MotivoViagemDBE Motivo { get; set; }

        public StatusViagemDBE ViagemStatus { get; set; }

        public DateTime DataInclusao { get; set; }

        public DateTime DataAlteracao { get; set; }

        public bool Status { get; set; }

        public ViagemDBE()
        {
            Motivo = new MotivoViagemDBE();
            ViagemStatus = new StatusViagemDBE();
            MotoristaViagem = new MotoristaDBE();
            VeiculoViagem = new VeiculoDBE();
        }
    }
}
