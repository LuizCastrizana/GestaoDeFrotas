using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Front.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Business.BLL
{
    internal class MotoristaBLL
    {
        private MotoristaDAL _dal;

        public MotoristaBLL()
        {
            _dal = new MotoristaDAL();
        }

        public void CadastrarMotorista (CadastroMotoristaVM dadosMotorista, ref Resposta<int> Resposta)
        {

        }
    }
}
