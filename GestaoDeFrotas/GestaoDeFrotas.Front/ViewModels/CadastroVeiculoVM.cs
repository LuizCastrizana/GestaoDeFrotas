using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;

namespace GestaoDeFrotas.Front.ViewModels
{
    public class CadastroVeiculoVM
    {
        public VeiculoVM VeiculoVM { get; set; }

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
            VeiculoVM = new VeiculoVM();
        }

        public CadastroVeiculoVM(VeiculoVM veiculo)
        {
            VeiculoVM = veiculo;
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

        public void CastFromDBE(VeiculoDBE obj)
        {
            ID = obj.ID;
            Placa = obj.Placa;
            Marca = obj.Marca;
            Modelo = obj.Modelo;
            Tipo = obj.Tipo;
            DataInclusao = obj.DataInclusao;
            DataAlteracao = obj.DataAlteracao;
            Status = obj.Status;
            ContagemMotoristas = new MotoristaDAL().ListByVeiculoID(obj.ID, true).Count();
        }

        public VeiculoDBE CastToDBE()
        {
            var VeiculoDBE = new VeiculoDBE
            {
                ID = this.ID,
                Placa = this.Placa,
                Marca = this.Marca,
                Modelo = this.Modelo,
                Tipo = this.Tipo,
                DataInclusao = this.DataInclusao,
                DataAlteracao = this.DataAlteracao,
                Status = this.Status,
            };

            return VeiculoDBE;
        }
    }
}