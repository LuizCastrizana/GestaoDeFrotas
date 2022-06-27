using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GestaoDeFrotas.Models;

namespace GestaoDeFrotas.Front.ViewModels
{
    public class DetalhesVeiculoVM
    {
        public int TipoDetalhe { get; set; }
        public int DetalheSelecionado { get; set; }
        public DetalheVeiculo Detalhe { get; set; }

        public DetalhesVeiculoVM()
        {
        }

    }
}