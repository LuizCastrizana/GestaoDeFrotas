using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CadastroDeCaminhoneiro.Models;
using GestaoDeFrotas.Data.DBENTITIES;

namespace CadastroDeCaminhoneiro.ViewModels
{
    public class PainelMotoristasVM
    {
        public string BuscaMotorista{ get; set; }
        public string OpcoesFiltragem { get; set; }
        public string OpcaoOrdenacao { get; set; }
        public bool Todos { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public IEnumerable<MotoristaDBE> Motoristas { get; set; }

        public PainelMotoristasVM()
        {
            this.OpcaoOrdenacao = "1";
            this.OpcoesFiltragem = "1";
            this.Todos = false;
        }
        public PainelMotoristasVM(string ordenacao, string filtragem, bool todos)
        {
            this.OpcaoOrdenacao = ordenacao;
            this.OpcoesFiltragem = filtragem;
            this.Todos = todos;
        }
    }
}