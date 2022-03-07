using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CadastroDeCaminhoneiro.Models;
using GestaoDeFrotas.Data.DBENTITIES;

namespace CadastroDeCaminhoneiro.ViewModels
{
    // TODO Adicionar campo de placa e transferir validação da model para cá
    public class CadastroVeiculoVM
    {
        public VeiculoDBE VeiculoDBE { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression(@"[A-Z]{3}-[0-9]{1}[A-Z0-9]{1}[0-9]{2}", ErrorMessage = "Valor inválido")]
        [Remote("VeiculoExiste", "Veiculo", AdditionalFields = "ID", ErrorMessage = "A placa inserida já foi cadastrada")]
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
}