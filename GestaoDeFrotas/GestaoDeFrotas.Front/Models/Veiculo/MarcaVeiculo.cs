using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace CadastroDeCaminhoneiro.Models
{
    public class MarcaVeiculo : OracleModel
    {
        public int ID { get; set; }
        [DisplayName("Marca")]
        public string Nome { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public bool Status { get; set; }
        public IEnumerable<MarcaVeiculo> ListarMarcas()
        {
            List<MarcaVeiculo> Marcas = new List<MarcaVeiculo>();

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
                            MarcaVeiculo marca = new MarcaVeiculo();
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

            List<MarcaVeiculo> ListaOrdenada = Marcas.OrderBy(o => o.Nome).ToList();

            return ListaOrdenada;
        }
        public MarcaVeiculo BuscarPorId(int id)
        {
            MarcaVeiculo resultado = new MarcaVeiculo();

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