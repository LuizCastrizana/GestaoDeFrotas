using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestaoDeFrotas.Models
{
    public class DetalheVeiculo : OracleModel
    {
        public int ID { get; set; }
        public string Descricao { get; set; }
        public int TipoDetalhe { get; set; }

        public void InserirDetalhe()
        {
            string comandoSql = "INSERT INTO ";
            switch (this.TipoDetalhe)
            {
                case 1:
                    comandoSql += "MARCAVEICULO (NOME) ";
                    break;
                case 2:
                    comandoSql += "MODELOVEICULO (NOME) ";
                    break;
                case 3:
                    comandoSql += "TIPOVEICULO (DESCRICAO) ";
                    break;
            }
            comandoSql += "VALUES "
                + "(\'" + this.Descricao + "\'" + ")";
            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(comandoSql, Conexao))
            {
                Conexao.Open();
                if (Comando.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Erro ao incluir detalhe de veículo");
                }
            }
        }
        public void ExcluirDetalhe()
        {
            string comandoSql = "DELETE FROM ";
            switch (this.TipoDetalhe)
            {
                case 1:
                    comandoSql += "MARCAVEICULO WHERE ID ";
                    break;
                case 2:
                    comandoSql += "MODELOVEICULO WHERE ID ";
                    break;
                case 3:
                    comandoSql += "TIPOVEICULO WHERE ID ";
                    break;
            }
            comandoSql += "= "
                + this.ID;
            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(comandoSql, Conexao))
            {
                Conexao.Open();
                if (Comando.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Erro ao excluir detalhe de veículo");
                }
            }
        }
        public IEnumerable<DetalheVeiculo> ListarDetalhe(int tipo)
        {
            List<DetalheVeiculo> listaDetalhes = new List<DetalheVeiculo>();
            string comandoSql = "SELECT ID, ";
            switch (tipo)
            {
                case 1:
                    comandoSql += "NOME FROM MARCAVEICULO ";
                    break;
                case 2:
                    comandoSql += "NOME FROM MODELOVEICULO ";
                    break;
                case 3:
                    comandoSql += "DESCRICAO FROM TIPOVEICULO ";
                    break;
            }
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
                            DetalheVeiculo detalhe = new DetalheVeiculo();
                            detalhe.ID = reader.GetInt32(0);
                            detalhe.Descricao = reader.GetString(1);
                            listaDetalhes.Add(detalhe);
                        }
                    }
                }
            }
            return listaDetalhes;
        }
    }
}