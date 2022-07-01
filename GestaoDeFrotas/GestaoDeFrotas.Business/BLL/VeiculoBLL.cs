using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Business.BLL
{
    public class VeiculoBLL
    {
        private readonly VeiculoDAL _veiculoDAL;

        public VeiculoBLL()
        {
            _veiculoDAL = new VeiculoDAL();
        }

        public void ListarVeiculosPorIDMotorista (int id, bool? status, ref RespostaNegocio<IEnumerable<VeiculoDBE>> Resposta)
        {
            Resposta.Retorno = _veiculoDAL.ListarVeiculosPorIDMotorista(id, status);
        }
    }
}
