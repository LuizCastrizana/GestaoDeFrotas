using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CadastroDeCaminhoneiro.Models;
using GestaoDeFrotas.Data.DBENTITIES;

namespace CadastroDeCaminhoneiro.ViewModels
{
    public class PainelVeiculosVM
    {
        public string BuscaPlaca { get; set; }
        public int OpcaoOrdenacao { get; set; }
        public int OpcoesFiltragem { get; set; }
        public bool Todos { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public IEnumerable<VeiculoDBE> Veiculos { get; set; }
        
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
    }
}