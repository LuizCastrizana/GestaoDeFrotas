using CadastroDeCaminhoneiro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CadastroDeCaminhoneiro
{
    public static class GeradorCodigoViagem
    {
        private const string CONTADORINICIAL = "00001";
        private const int CONTADORMAXIMO = 99999;
        public static void GerarCodigo(Viagem viagem)
        {
            if(viagem.Motivo.ID == 0)
                throw new Exception("Não foi possível gerar um novo código de viagem: Motivo não informado.");

            // Inicia o código com as duas primeiras letras do motivo e a data atual
            viagem.Codigo =
                viagem.Motivo.Descricao.Substring(0, 2).ToUpper() +
                DateTime.Now.ToString("yy") +
                DateTime.Now.ToString("dd") +
                DateTime.Now.ToString("MM");

            // Busca viagens geradas na data atual e com o mesmo motivo, pois nesse caso o início do código será igual ao cógigo que está sendo gerado
            var viagemBusca = new Viagem()
            {
                Motivo = viagem.Motivo
            };
            var viagensDia = viagemBusca.Read().Where(m=>m.DataInclusao.Date == DateTime.Now.Date);

            // Se houver qualquer viagem gerada na data atual e com o mesmo motivo, verifica o valor do contador do código da viagem mais recente e incrementa o valor
            // Em seguida concatena o valor do contador ao código da viagem
            if (viagensDia.Any())
            {
                var ultimaViagem = viagensDia.OrderByDescending(v => v.ID).First();
                var contador = Convert.ToInt32(ultimaViagem.Codigo.Substring(8, 5));
                if (contador == CONTADORMAXIMO)
                    throw new Exception("Não foi possível gerar um novo código de viagem: limite excedido.");
                contador++;
                viagem.Codigo += contador.ToString("D5");
            }
            // Se não, concatena o contador inicial
            else
                viagem.Codigo += CONTADORINICIAL;
        }
    }
}