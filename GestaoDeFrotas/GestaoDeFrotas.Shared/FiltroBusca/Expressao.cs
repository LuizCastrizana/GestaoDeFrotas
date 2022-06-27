using GestaoDeFrotas.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Shared.FiltroBusca
{
    public class Expressao
    {
        public EnumOperadorLogico OperadorLogico { get; set; }
        public List<Condicao> ListaCondicao { get; set; }

    }
}
