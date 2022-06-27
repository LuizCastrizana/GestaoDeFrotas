using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GestaoDeFrotas.Data.DBENTITIES;

namespace GestaoDeFrotas.Front.ViewModels
{
    public class PainelVeiculosVM
    {
        public string BuscaPlaca { get; set; }
        public int OpcaoOrdenacao { get; set; }
        public int OpcoesFiltragem { get; set; }
        public bool Todos { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public IEnumerable<VeiculoVM> Veiculos { get; set; }

        public PainelVeiculosVM()
        {
            OpcoesFiltragem = 1;
            OpcaoOrdenacao = 1;
            Todos = false;
            BuscaPlaca = "";
        }
        public PainelVeiculosVM(int filtragem, int ordenacao, bool todos)
        {
            OpcoesFiltragem = filtragem;
            OpcaoOrdenacao = ordenacao;
            Todos = todos;
        }

        public void CastListaVeiculosParaVM(IEnumerable<VeiculoDBE> obj) 
        {
            var lista = new List<VeiculoVM>();
            foreach (var item in obj)
            {
                var VeiculoVM = new VeiculoVM();
                VeiculoVM.CastFromDBE(item);
                lista.Add(VeiculoVM);
            }
            Veiculos = lista;
        }
    }
}