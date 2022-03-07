using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroDeCaminhoneiro.Models
{
    interface IModel
    {
        int ID { get; set; }
        DateTime DataInclusao { get; set; }
        DateTime DataAlteracao { get; set; }
        bool Status { get; set; }

        void Create();
        void Read(int id);
        void Update();
        void UpdateStatus(int id, bool status);
    }
}
