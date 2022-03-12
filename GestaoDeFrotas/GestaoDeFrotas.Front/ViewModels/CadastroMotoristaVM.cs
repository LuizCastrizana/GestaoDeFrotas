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
    public class CadastroMotoristaVM
    {
        public MotoristaVM Motorista { get; set; }
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
            Motorista = new MotoristaVM();
        }

        /// <summary>
        /// Insere o valor dos parametros <see cref="MunicipioSelecionado"/>, <see cref="CategoriaSelecionada"/>, <see cref="DataNascimento"/>, <see cref="DataEmissao"/>, <see cref="DataValidade"/>
        /// nos campos equivalentes dentro do objeto <see cref="Motorista"/>. 
        /// </summary>
        public void CastToMotoristaVM()
        {
            Motorista.Endereco.Municipio.ID = Convert.ToInt32(MunicipioSelecionado);
            Motorista.CNH.Categoria.ID = Convert.ToInt32(CategoriaSelecionada);
            Motorista.DataNascimento = StringTools.ConverterEmData(DataNascimento, "en-US");
            Motorista.CNH.DataEmissao = StringTools.ConverterEmData(DataEmissao, "en-US");
            Motorista.CNH.DataValidade = StringTools.ConverterEmData(DataValidade, "en-US");
        }
    }

    public class MotoristaVM
    {
        [DisplayName("ID")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(50)]
        [RegularExpression(@"[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ ]+", ErrorMessage = "Utilize apenas letras")]
        [DisplayName("Nome")]
        public string PrimeiroNome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(50)]
        [RegularExpression(@"[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ ]+", ErrorMessage = "Utilize apenas letras")]
        [DisplayName("Sobrenome")]
        public string Sobrenome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Data de nascimento")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}\-\d{2}$", ErrorMessage = "CPF inválido")]
        [Remote("ValidaCpf", "Motorista", AdditionalFields = "ID")]
        [DisplayName("CPF")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("RG")]
        public string RG { get; set; }

        public CNHVM CNH { get; set; }

        public IEnumerable<VeiculoDBE> ListaVeiculos { get; set; }

        public EnderecoVM Endereco { get; set; }

        [DisplayName("Data de inclusão")]
        public DateTime DataInclusao { get; set; }

        [DisplayName("Data de alteração")]
        public DateTime DataAlteracao { get; set; }

        [DisplayName("Veículos")]
        public int ContagemVeiculos { get; set; }

        [DisplayName("Ativo")]
        public bool Status { get; set; }

        public MotoristaVM()
        {
            Endereco = new EnderecoVM();
            CNH = new CNHVM();
            ListaVeiculos = Enumerable.Empty<VeiculoDBE>();
        }

        public MotoristaDBE CastToDBE()
        {
            var motoristaDBE = new MotoristaDBE();

            motoristaDBE.ID = ID;
            motoristaDBE.PrimeiroNome = PrimeiroNome;
            motoristaDBE.Sobrenome = Sobrenome;
            motoristaDBE.CPF = CPF;
            motoristaDBE.RG = RG;
            motoristaDBE.DataNascimento = DataNascimento;

            motoristaDBE.CNH.ID = CNH.ID;
            motoristaDBE.CNH.Numero = CNH.Numero;
            motoristaDBE.CNH.RENACH = CNH.RENACH;
            motoristaDBE.CNH.Espelho = CNH.Espelho;
            motoristaDBE.CNH.DataEmissao = CNH.DataEmissao;
            motoristaDBE.CNH.DataValidade = CNH.DataValidade;
            motoristaDBE.CNH.Categoria.ID = CNH.Categoria.ID;

            motoristaDBE.Endereco.ID = Endereco.ID;
            motoristaDBE.Endereco.Logradouro = Endereco.Logradouro;
            motoristaDBE.Endereco.Numero = Endereco.Numero;
            motoristaDBE.Endereco.Complemento = Endereco.Complemento;
            motoristaDBE.Endereco.Cep = Endereco.Cep;
            motoristaDBE.Endereco.Bairro = Endereco.Bairro;
            motoristaDBE.Endereco.Municipio.ID = Endereco.Municipio.ID;

            return motoristaDBE;
        }

        public void CastFromDBE(MotoristaDBE dbe)
        {
            
            ID = dbe.ID;
            PrimeiroNome = dbe.PrimeiroNome;
            Sobrenome = dbe.Sobrenome;
            CPF = dbe.CPF;
            RG = dbe.RG;
            CPF = dbe.CPF;
            DataNascimento = dbe.DataNascimento;
            DataInclusao = dbe.DataInclusao;
            DataAlteracao = dbe.DataAlteracao;
            Status = dbe.Status;

            ContagemVeiculos = new VeiculoDAL().ListarVeiculosPorIDMotorista(ID, null).Count();

            CNH.ID = dbe.CNH.ID;
            CNH.Numero = dbe.CNH.Numero;
            CNH.RENACH = dbe.CNH.RENACH;
            CNH.Espelho = dbe.CNH.Espelho;
            CNH.DataEmissao = dbe.CNH.DataEmissao;
            CNH.DataValidade = dbe.CNH.DataValidade;
            CNH.Categoria.ID = dbe.CNH.Categoria.ID;
            CNH.Categoria.Categoria = dbe.CNH.Categoria.Categoria;

            Endereco.ID = dbe.Endereco.ID;
            Endereco.Logradouro = dbe.Endereco.Logradouro;
            Endereco.Numero = dbe.Endereco.Numero;
            Endereco.Complemento = dbe.Endereco.Complemento;
            Endereco.Cep = dbe.Endereco.Cep;
            Endereco.Bairro = dbe.Endereco.Bairro;
            Endereco.Municipio.ID = dbe.Endereco.Municipio.ID;
            Endereco.Municipio.NomeMunicipio = dbe.Endereco.Municipio.NomeMunicipio;
            Endereco.Municipio.Estado.ID = dbe.Endereco.Municipio.Estado.ID;
            Endereco.Municipio.Estado.NomeEstado = dbe.Endereco.Municipio.Estado.NomeEstado;
            Endereco.Municipio.Estado.UF = dbe.Endereco.Municipio.Estado.UF;

        }
    }

    public class CNHVM
    {
        [DisplayName("ID")]
        public int ID { get; set; }

        [StringLength(11)]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "Inserir 11 dígitos, apenas números")]
        [DisplayName("Número registro")]
        public string Numero { get; set; }

        [StringLength(11)]
        [RegularExpression(@"^[A-Za-z]{2}[0-9]{9}$", ErrorMessage = "RENACH Inválido")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("RENACH")]
        public string RENACH { get; set; }

        [StringLength(10)]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Inserir 10 dígitos, apenas números")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Espelho")]
        public string Espelho { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Data de emissão")]
        public DateTime DataEmissao { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Data de vencimento")]
        public DateTime DataValidade { get; set; }

        public CategoriaCNHDBE Categoria { get; set; }

        [DisplayName("Data de inclusão")]
        public DateTime DataInclusao { get; set; }

        [DisplayName("Data de alteração")]
        public DateTime DataAlteracao { get; set; }

        [DisplayName("Status")]
        public bool Status { get; set; }

        public CNHVM()
        {
            this.Categoria = new CategoriaCNHDBE();
        }
    }

    public class EnderecoVM
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(100)]
        [DisplayName("Logradouro")]
        public string Logradouro { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#}")]
        [Range(0, int.MaxValue, ErrorMessage = "Valor Inválido")]
        [DisplayName("Número")]
        public int? Numero { get; set; }

        [StringLength(20)]
        [DisplayName("Complemento")]
        public string Complemento { get; set; }

        [StringLength(50)]
        [DisplayName("Bairro")]
        public string Bairro { get; set; }

        public MunicipioDBE Municipio { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression(@"[0-9]{5}-[0-9]{3}", ErrorMessage = "Valor inválido")]
        [DisplayName("CEP")]
        public string Cep { get; set; }

        [DisplayName("Data de inclusão")]
        public DateTime DataInclusao { get; set; }

        [DisplayName("Data de alteração")]
        public DateTime DataAlteracao { get; set; }

        [DisplayName("Status")]
        public bool Status { get; set; }

        public EnderecoVM()
        {
            this.Municipio = new MunicipioDBE();
            this.Complemento = "";
            this.Bairro = "";
        }
    }
}
