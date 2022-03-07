using GestaoDeFrotas.Data.DBENTITIES;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoDeFrotas.Data.DAL
{
    public class MarcaVeiculoDAL : OracleBaseDAL
    {

        public IEnumerable<MarcaVeiculoDBE> ListarMarcas()
        {
            List<MarcaVeiculoDBE> Marcas = new List<MarcaVeiculoDBE>();

            string comandoSql =
                "SELECT " +
                "M.ID, " +
                "M.NOME, " +
                "M.DATAINCLUSAO, " +
                "M.DATAALTERACAO, " +
                "M.STATUS " +
                "FROM " +
                "MARCAVEICULO M ";

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
                            MarcaVeiculoDBE marca = new MarcaVeiculoDBE();
                            marca.ID = reader.GetInt32(0);
                            marca.Nome = reader.GetString(1);
                            marca.DataInclusao = reader.GetDateTime(2);
                            if (!reader.IsDBNull(3))
                                marca.DataAlteracao = reader.GetDateTime(3);
                            marca.Status = Convert.ToBoolean(reader.GetInt32(4));
                            Marcas.Add(marca);
                        }
                    }
                }
            }

            List<MarcaVeiculoDBE> ListaOrdenada = Marcas.OrderBy(o => o.Nome).ToList();

            return ListaOrdenada;
        }

        public MarcaVeiculoDBE BuscarPorId(int id)
        {
            MarcaVeiculoDBE resultado = new MarcaVeiculoDBE();

            string comandoSql = "SELECT ID, " +
                                "NOME " +
                                "FROM MARCAVEICULO " +
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
                        resultado.Nome = reader.GetString(1);
                    }
                }
            }

            return resultado;
        }

    }
}