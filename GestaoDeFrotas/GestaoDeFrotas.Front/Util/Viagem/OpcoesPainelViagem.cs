using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestaoDeFrotas
{
    public class OpcoesPainelViagem
    {
        public List<DropDownItem> Ordenacao { get; set; }
        public List<DropDownItem> Filtros { get; set; }
        public List<DropDownItem> CampoOrdenacao { get; set; }
        public List<DropDownItem> StatusViagem { get; set; }
    }
}