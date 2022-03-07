using GestaoDeFrotas.Data.DBENTITIES;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoDeFrotas.Data.DAL
{
    public class TipoVeiculoDAL : OracleBaseDAL
    {
        public IEnumerable<TipoVeiculoDBE> ListarTipos()
        {
            List<TipoVeiculoDBE> ListaTipos = new List<TipoVeiculoDBE>();

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
                            TipoVeiculoDBE tipoVeiculo = new TipoVeiculoDBE();
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

            List<TipoVeiculoDBE> ListaOrdenada = ListaTipos.OrderBy(o => o.Descricao).ToList();
            return ListaOrdenada;
        }
        public TipoVeiculoDBE BuscarPorId(int id)
        {
            TipoVeiculoDBE resultado = new TipoVeiculoDBE();

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