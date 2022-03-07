using GestaoDeFrotas.Data.DBENTITIES;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoDeFrotas.Data.DAL
{
    public class ModeloVeiculoDAL : OracleBaseDAL
    {
        public IEnumerable<ModeloVeiculoDBE> ListarModelos()
        {
            List<ModeloVeiculoDBE> Modelos = new List<ModeloVeiculoDBE>();

            string comandoSql =
                "SELECT " +
                "M.ID, " +
                "M.NOME, " +
                "M.DATAINCLUSAO, " +
                "M.DATAALTERACAO, " +
                "M.STATUS " +
                "FROM " +
                "MODELOVEICULO M ";

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
                            ModeloVeiculoDBE modelo = new ModeloVeiculoDBE();
                            modelo.ID = reader.GetInt32(0);
                            modelo.Nome = reader.GetString(1);
                            modelo.DataInclusao = reader.GetDateTime(2);
                            if (!reader.IsDBNull(3))
                                modelo.DataAlteracao = reader.GetDateTime(3);
                            modelo.Status = Convert.ToBoolean(reader.GetInt32(4));
                            Modelos.Add(modelo);
                        }
                    }
                }
            }

            return Modelos.OrderBy(o => o.Nome).ToList();
        }
        public ModeloVeiculoDBE BuscarPorId(int id)
        {
            ModeloVeiculoDBE resultado = new ModeloVeiculoDBE();

            string comandoSql = "SELECT ID, " +
                                "NOME " +
                                "FROM MODELOVEICULO " +
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