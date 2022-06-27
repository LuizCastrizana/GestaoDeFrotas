using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GestaoDeFrotas.Data.DBENTITIES;
using GestaoDeFrotas.Data.DAL;

namespace GestaoDeFrotas.Front.ViewModels
{
    public class CadastroViagemVM
    {
        public ViagemDBE Viagem { get; set; }
        public string BuscaMotorista { get; set; }

        public int VeiculoID { get; set; }

        public int MotoristaID { get; set; }

        [DisplayName("Motivo")]
        public int MotivoID { get; set; }

        [DisplayName("Motivo")]
        public string Motivo { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Início")]
        public string Inicio { get; set; }

        [DisplayName("Fim")]
        public string Fim { get; set; }

        [DisplayName("Nome")]
        public string PrimeiroNome { get; set; }

        [DisplayName("Sobrenome")]
        public string Sobrenome { get; set; }

        [DisplayName("CPF")]
        public string CPF { get; set; }

        [DisplayName("CNH")]
        public string CNH { get; set; }

        [DisplayName("Placa")]
        public string Placa { get; set; }

        [DisplayName("Marca")]
        public string Marca { get; set; }

        [DisplayName("Modelo")]
        public string Modelo { get; set; }

        [DisplayName("Tipo")]
        public string Tipo { get; set; }

        [DisplayName("Código")]
        public string Codigo { get; set; }

        [DisplayName("Status")]
        public string StatusViagem { get; set; }


        public CadastroViagemVM()
        {
            Viagem = new ViagemDBE();
        }

        public void DBEToVM()
        {
            Inicio = Viagem.Inicio.ToString("dd/MM/yyyy HH:mm");
            Fim = Viagem.Fim.ToString("dd/MM/yyyy HH:mm");
            StatusViagem = Viagem.ViagemStatus.Descricao;
            Motivo = Viagem.Motivo.Descricao;
            MotoristaID = Viagem.MotoristaViagem.ID;
            VeiculoID = Viagem.VeiculoViagem.ID;
            PrimeiroNome = Viagem.MotoristaViagem.PrimeiroNome;
            Sobrenome = Viagem.MotoristaViagem.Sobrenome;
            CPF = Viagem.MotoristaViagem.CPF;
            CNH = Viagem.MotoristaViagem.CNH.Numero;
            Placa = Viagem.VeiculoViagem.Placa;
            Modelo = Viagem.VeiculoViagem.Modelo.Nome;
            Marca = Viagem.VeiculoViagem.Marca.Nome;
            Tipo = Viagem.VeiculoViagem.Tipo.Descricao;
        }

        public void VmToDBE()
        {
            Viagem.Inicio = StringTools.ConverterEmData(Inicio, "en-US");
            Viagem.MotoristaViagem.ID = MotoristaID;
            Viagem.VeiculoViagem.ID = VeiculoID;
            Viagem.Motivo = new MotivoViagemDAL().Read(MotivoID);
        }

    }

    public class ViagemVM
    {
        public int ID { get; set; }

        [DisplayName("Código da viagem")]
        public string Codigo { get; set; }

        public MotoristaDBE MotoristaViagem { get; set; }

        public VeiculoDBE VeiculoViagem { get; set; }

        [DisplayName("Início")]
        public DateTime Inicio { get; set; }

        [DisplayName("Fim")]
        public DateTime Fim { get; set; }

        public MotivoViagemDBE Motivo { get; set; }

        public StatusViagemDBE ViagemStatus { get; set; }

        [DisplayName("Data de inclusão")]
        public DateTime DataInclusao { get; set; }

        [DisplayName("Data de alteração")]
        public DateTime DataAlteracao { get; set; }

        [DisplayName("Status")]
        public bool Status { get; set; }

        public ViagemVM()
        {
            Motivo = new MotivoViagemDBE();
            ViagemStatus = new StatusViagemDBE();
            MotoristaViagem = new MotoristaDBE();
            VeiculoViagem = new VeiculoDBE();
        }
    }

}