using GestaoDeFrotas.Data.DBENTITIES;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoDeFrotas.Data.DAL
{
    public class EstadoDAL : OracleBaseDAL
    {
        public IEnumerable<EstadoDBE> List()
        {
            List<EstadoDBE> Estados = new List<EstadoDBE>();
            string comandoSql =
                "SELECT " +
                "E.ID, " +
                "E.NOME, " +
                "E.UF, " +
                "E.CODIGOIBGE, " +
                "E.DATAINCLUSAO, " +
                "E.DATAALTERACAO, " +
                "E.STATUS " +
                "FROM " +
                "ESTADO E ";

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();
                Comando.CommandText = comandoSql;
                using(var reader = Comando.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            EstadoDBE estado = new EstadoDBE();
                            estado.ID = reader.GetInt32(0);
                            estado.NomeEstado = reader.GetString(1).ToUpper();
                            estado.UF = reader.GetString(2);
                            estado.CodigoIbge = reader.GetString(3);
                            estado.DataInclusao = reader.GetDateTime(4);
                            if (!reader.IsDBNull(5))
                                estado.DataAlteracao = reader.GetDateTime(5);
                            estado.Status = Convert.ToBoolean(reader.GetInt32(6));
                            Estados.Add(estado);
                        }
                    }
                }
            }

            List<EstadoDBE> ListaOrdenada = Estados.OrderBy(o => o.NomeEstado).ToList();
            return ListaOrdenada;
        }
        public void Create(EstadoDBE obj)
        {
            string comandoSql = "INSERT INTO ESTADO (ID, NOME, UF, CODIGOIBGE) VALUES ("
                + obj.ID
                + ", \'" + obj.NomeEstado + "\'"
                + ", \'" + obj.UF + "\'"
                + ", \'" + obj.CodigoIbge + "\'" + ")";
            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();
                Comando.CommandText = comandoSql;
                Comando.ExecuteNonQuery();
            }
        }
    }
}