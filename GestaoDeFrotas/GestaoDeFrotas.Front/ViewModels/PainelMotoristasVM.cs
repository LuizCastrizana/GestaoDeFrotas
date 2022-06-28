using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GestaoDeFrotas.Data.DBENTITIES;
using GestaoDeFrotas.Enums;

namespace GestaoDeFrotas.Front.ViewModels
{
    public class PainelMotoristasVM
    {
        public string BuscaMotorista{ get; set; }
        public int OpcoesFiltragem { get; set; }
        public int OpcaoOrdenacao { get; set; }
        public int OpcaoCampoOrdenacao { get; set; }
        public bool Todos { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public IEnumerable<MotoristaVM> Motoristas { get; set; }

        public PainelMotoristasVM()
        {
            this.OpcaoOrdenacao = (int)ENUMOPCOESORDENACAO.CRESCENTE;
            this.OpcoesFiltragem = (int)ENUMCAMPOSPAIELMOTORISTAS.NOME;
            this.Todos = false;
        }
        public PainelMotoristasVM(int ordenacao, int filtragem, bool todos)
        {
            this.OpcaoOrdenacao = ordenacao;
            this.OpcoesFiltragem = filtragem;
            this.Todos = todos;
        }
        public void CastListaMotoristasParaVM(IEnumerable<MotoristaDBE> obj)
        {
            var lista = new List<MotoristaVM>();
            foreach (var item in obj)
            {
                var MotoristaVM = new MotoristaVM();
                MotoristaVM.CastFromDBE(item);
                lista.Add(MotoristaVM);
            }
            Motoristas = lista;
        }
    }
}