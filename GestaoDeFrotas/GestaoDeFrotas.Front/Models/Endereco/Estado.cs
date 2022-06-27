using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace GestaoDeFrotas.Models
{
    public class Estado : OracleModel
    {
        public int ID { get; set; }
        [DisplayName("Estado")]
        public string NomeEstado { get; set; }
        [DisplayName("UF")]
        public string UF { get; set; }
        [DisplayName("Codigo IBGE")]
        public string CodigoIbge { get; set; }
        [DisplayName("Data de inclusão")]
        public DateTime DataInclusao { get; set; }
        [DisplayName("Data de alteração")]
        public DateTime DataAlteracao { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        public IEnumerable<Estado> List()
        {
            List<Estado> Estados = new List<Estado>();
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
                            Estado estado = new Estado();
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

            List<Estado> ListaOrdenada = Estados.OrderBy(o => o.NomeEstado).ToList();
            return ListaOrdenada;
        }
        public void Create()
        {
            string comandoSql = "INSERT INTO ESTADO (ID, NOME, UF, CODIGOIBGE) VALUES ("
                + this.ID
                + ", \'" + this.NomeEstado + "\'"
                + ", \'" + this.UF + "\'"
                + ", \'" + this.CodigoIbge + "\'" + ")";
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