using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;

namespace CadastroDeCaminhoneiro.Models
{
    public class StatusViagem : OracleModel, IModel
    {
        public int ID { get; set; }
        [DisplayName("Status")]
        public string Descricao { get; set; }
        [DisplayName("Data de inclusão")]
        public DateTime DataInclusao { get; set; }
        [DisplayName("Data de alteração")]
        public DateTime DataAlteracao { get; set; }
        [DisplayName("Ativa")]
        public bool Status { get; set; }

        public StatusViagem()
        {

        }
        public IEnumerable<StatusViagem> List()
        {
            List<StatusViagem> lista = new List<StatusViagem>();
            string textoComando = "SELECT ID, DESCRICAO, DATAINCLUSAO, DATAALTERACAO, STATUS FROM STATUSVIAGEM";
            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();
                Comando.CommandText = textoComando;
                using (var DataReader = Comando.ExecuteReader())
                {
                    if (DataReader.HasRows)
                    {
                        while (DataReader.Read())
                        {
                            var itemLista = new StatusViagem();
                            itemLista.ID = Convert.ToInt32(DataReader["ID"]);
                            itemLista.Descricao = Convert.ToString(DataReader["DESCRICAO"]);
                            itemLista.DataInclusao = Convert.ToDateTime(DataReader["DATAINCLUSAO"]);
                            if (DataReader["DATAALTERACAO"] != DBNull.Value)
                                itemLista.DataAlteracao = Convert.ToDateTime(DataReader["DATAALTERACAO"]);
                            itemLista.Status = Convert.ToBoolean(DataReader["STATUS"]);
                            lista.Add(itemLista);
                        }
                    }
                }
            }
            return lista;
        }
        public void Read(int id)
        {
            string textoComando = "SELECT ID, DESCRICAO, DATAINCLUSAO, DATAALTERACAO, STATUS FROM STATUSVIAGEM WHERE ID = :ID";
            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();
                Comando.CommandText = textoComando;
                Comando.Parameters.Add("ID", id);
                using (var DataReader = Comando.ExecuteReader())
                {
                    if (DataReader.HasRows)
                    {
                        DataReader.Read();
                        ID = Convert.ToInt32(DataReader["ID"]);
                        Descricao = Convert.ToString(DataReader["DESCRICAO"]);
                        DataInclusao = Convert.ToDateTime(DataReader["DATAINCLUSAO"]);
                        if (DataReader["DATAALTERACAO"] != DBNull.Value)
                            DataAlteracao = Convert.ToDateTime(DataReader["DATAALTERACAO"]);
                        Status = Convert.ToBoolean(DataReader["STATUS"]);
                    }
                }
            }
        }

        public void Create()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(int id, bool status)
        {
            throw new NotImplementedException();
        }
    }
}