using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CadastroDeCaminhoneiro.Models;
using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;

namespace CadastroDeCaminhoneiro.ViewModels
{
    public class CadastroVeiculoVM
    {
        public VeiculoDBE VeiculoDBE { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression(@"[A-Z]{3}-[0-9]{1}[A-Z0-9]{1}[0-9]{2}", ErrorMessage = "Valor inválido")]
        [Remote("ValidaVeiculoAtivo", "Veiculo")]
        [DisplayName("Placa")]
        public string Placa { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Marca")]
        public int MarcaID { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Modelo")]
        public int ModeloID { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Tipo")]
        public int TipoID { get; set; }

        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}\-\d{2}$", ErrorMessage = "CPF inválido")]
        public string BuscaMotorista { get; set; }

        [DisplayName("Data de inclusão")]
        public string DataInclusao { get; set; }

        [DisplayName("Data de alteração")]
        public string DataAlteracao { get; set; }


        public CadastroVeiculoVM()
        {
            VeiculoDBE = new VeiculoDBE();
        }

        public CadastroVeiculoVM(VeiculoDBE veiculo)
        {
            VeiculoDBE = veiculo;
        }
    }

    public class VeiculoVM
    {
        public int ID { get; set; }

        public string Placa { get; set; }

        public MarcaVeiculoDBE Marca { get; set; }

        public ModeloVeiculoDBE Modelo { get; set; }

        public TipoVeiculoDBE Tipo { get; set; }

        public IEnumerable<MotoristaDBE> ListaMotoristas { get; set; }

        public DateTime DataInclusao { get; set; }

        public DateTime DataAlteracao { get; set; }

        public bool Status { get; set; }
        public int ContagemMotoristas { get; set; }

        public VeiculoVM()
        {
            Marca = new MarcaVeiculoDBE();
            Modelo = new ModeloVeiculoDBE();
            Tipo = new TipoVeiculoDBE();
            ListaMotoristas = Enumerable.Empty<MotoristaDBE>();
        }

        public VeiculoVM CastFromDBE(VeiculoDBE obj)
        {
            VeiculoVM vm = new VeiculoVM
            {
                ID = obj.ID,
                Placa = obj.Placa,
                Marca = obj.Marca,
                Modelo = obj.Modelo,
                Tipo = obj.Tipo,
                DataInclusao = obj.DataInclusao,
                DataAlteracao = obj.DataAlteracao,
                Status = obj.Status,
                ContagemMotoristas = new MotoristaDAL().ListByVeiculoID(obj.ID, true).Count()
            };

            return vm;
        }
    }
}