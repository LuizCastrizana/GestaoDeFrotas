using CadastroDeCaminhoneiro.Models;
using GestaoDeFrotas.Data.DBENTITIES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CadastroDeCaminhoneiro.ViewModels
{
    public class PainelViagensVM
    {
        public IEnumerable<ViagemDBE> Viagens { get; set; }
        public string BuscaViagem { get; set; }
        public int OpcoesFiltragem { get; set; }
        public int OpcaoOrdenacao { get; set; }
        public int OpcaoCampoOrdenacao{ get; set; }
        public int StatusViagem { get; set; }
        public bool Todos { get; set; }
        public PainelViagensVM()
        {
            Viagens = Enumerable.Empty<ViagemDBE>();
            BuscaViagem = string.Empty;
            Todos = false;
        }
    }
}