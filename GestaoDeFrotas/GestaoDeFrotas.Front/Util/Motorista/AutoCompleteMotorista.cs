using CadastroDeCaminhoneiro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CadastroDeCaminhoneiro
{
    public class AutoCompleteMotorista
    {
        public string Nome { get; set; }
        public string CPF { get; set; }

        public AutoCompleteMotorista()
        {

        }
        public AutoCompleteMotorista(Motorista motorista)
        {
            Nome = motorista.CPF + " / " + motorista.PrimeiroNome + " " + motorista.Sobrenome;
            CPF = motorista.CPF;
        }
    }
}