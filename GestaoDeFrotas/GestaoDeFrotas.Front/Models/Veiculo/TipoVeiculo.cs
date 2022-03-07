using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace CadastroDeCaminhoneiro.Models
{
    public class TipoVeiculo : OracleModel
    {
        public int ID { get; set; }
        [DisplayName("Tipo")]
        public string Descricao { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public bool Status { get; set; }

        public IEnumerable<TipoVeiculo> ListarTipos()
        {
            List<TipoVeiculo> ListaTipos = new List<TipoVeiculo>();

            string comandoSql =
                "SELECT " +
                "E.ID, " +
                "E.DESCRICAO, " +
                "E.DATAINCLUSAO, " +
                "E.DATAALTERACAO, " +
                "E.STATUS " +
                "FROM " +
                "TIPOVEICULO E ";

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(comandoSql, Conexao))
            {
                Conexao.Open();
                using (var reader = Comando.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            TipoVeiculo tipoVeiculo = new TipoVeiculo();
                            tipoVeiculo.ID = reader.GetInt32(0);
                            tipoVeiculo.Descricao = reader.GetString(1);
                            tipoVeiculo.DataInclusao = reader.GetDateTime(2);
                            if (!reader.IsDBNull(3))
                                tipoVeiculo.DataAlteracao = reader.GetDateTime(3);
                            tipoVeiculo.Status = Convert.ToBoolean(reader.GetInt32(4));
                            ListaTipos.Add(tipoVeiculo);
                        }
                    }
                }
            }

            List<TipoVeiculo> ListaOrdenada = ListaTipos.OrderBy(o => o.Descricao).ToList();
            return ListaOrdenada;
        }
        public TipoVeiculo BuscarPorId(int id)
        {
            TipoVeiculo resultado = new TipoVeiculo();

            string comandoSql = "SELECT ID, " +
                                "DESCRICAO " +
                                "FROM TIPOVEICULO " +
                                "WHERE ID = " + id.ToString();

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(comandoSql, Conexao))
            {
                Conexao.Open();
                using (var reader = Comando.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        resultado.ID = reader.GetInt32(0);
                        resultado.Descricao = reader.GetString(1);
                    }
                }
            }

            return resultado;
        }
    }
}