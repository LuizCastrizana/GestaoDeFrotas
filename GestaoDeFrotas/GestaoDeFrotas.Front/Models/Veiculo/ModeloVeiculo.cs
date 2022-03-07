using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace CadastroDeCaminhoneiro.Models
{
    public class ModeloVeiculo : OracleModel
    {
        public int ID { get; set; }
        [DisplayName("Modelo")]
        public string Nome { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public bool Status { get; set; }
        
        public IEnumerable<ModeloVeiculo> ListarModelos()
        {
            List<ModeloVeiculo> Modelos = new List<ModeloVeiculo>();

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
                            ModeloVeiculo modelo = new ModeloVeiculo();
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
        public ModeloVeiculo BuscarPorId(int id)
        {
            ModeloVeiculo resultado = new ModeloVeiculo();

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