using GestaoDeFrotas.Data.DBENTITIES;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GestaoDeFrotas.Data.DAL
{
    public class StatusViagemDAL : OracleBaseDAL, IDal<StatusViagemDBE>
    {
        public IEnumerable<StatusViagemDBE> List()
        {
            List<StatusViagemDBE> lista = new List<StatusViagemDBE>();
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
                            var itemLista = new StatusViagemDBE();
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
        public StatusViagemDBE Read(int id)
        {
            var retorno = new StatusViagemDBE();

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
                        retorno.ID = Convert.ToInt32(DataReader["ID"]);
                        retorno.Descricao = Convert.ToString(DataReader["DESCRICAO"]);
                        retorno.DataInclusao = Convert.ToDateTime(DataReader["DATAINCLUSAO"]);
                        if (DataReader["DATAALTERACAO"] != DBNull.Value)
                            retorno.DataAlteracao = Convert.ToDateTime(DataReader["DATAALTERACAO"]);
                        retorno.Status = Convert.ToBoolean(DataReader["STATUS"]);
                    }
                }
            }

            return retorno;
        }

        public void Create(StatusViagemDBE obj)
        {
            throw new NotImplementedException();
        }

        public void Update(StatusViagemDBE obj)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(int id, bool status)
        {
            throw new NotImplementedException();
        }
    }
}