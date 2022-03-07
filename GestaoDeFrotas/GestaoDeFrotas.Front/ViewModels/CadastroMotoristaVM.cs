using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CadastroDeCaminhoneiro.Models;

namespace CadastroDeCaminhoneiro.ViewModels
{
    public class CadastroMotoristaVM
    {
        public Motorista Motorista { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Estado")]
        public string EstadoSelecionado { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Município")]
        public string MunicipioSelecionado { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Categoria")]
        public string CategoriaSelecionada { get; set; }
        public string BuscaPlaca { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Data de nascimento")]
        public string DataNascimento { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Data de emissão")]
        public string DataEmissao { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Data de validade")]
        public string DataValidade { get; set; }

        public CadastroMotoristaVM()
        {
            Motorista = new Motorista();
        }

        /// <summary>
        /// Insere o valor dos parametros <see cref="MunicipioSelecionado"/>, <see cref="CategoriaSelecionada"/>, <see cref="DataNascimento"/>, <see cref="DataEmissao"/>, <see cref="DataValidade"/>
        /// nos campos equivalentes dentro do objeto <see cref="Motorista"/>. 
        /// </summary>
        public void VmToModel()
        {
            Motorista.Endereco.Municipio.ID = Convert.ToInt32(MunicipioSelecionado);
            Motorista.CNH.Categoria.ID = Convert.ToInt32(CategoriaSelecionada);
            Motorista.DataNascimento = StringTools.ConverterEmData(DataNascimento, "en-US");
            Motorista.CNH.DataEmissao = StringTools.ConverterEmData(DataEmissao, "en-US");
            Motorista.CNH.DataValidade = StringTools.ConverterEmData(DataValidade, "en-US");
        }
    }
}
