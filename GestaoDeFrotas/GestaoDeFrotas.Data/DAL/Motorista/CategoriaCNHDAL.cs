using GestaoDeFrotas.Data.DBENTITIES;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoDeFrotas.Data.DAL
{
    public class CategoriaCNHDAL : OracleBaseDAL
    {
        public IEnumerable<CategoriaCNHDBE> List()
        {
            List<CategoriaCNHDBE> lista = new List<CategoriaCNHDBE>();
            IEnumerable<CategoriaCNHDBE> listaOrdenada = new List<CategoriaCNHDBE>();
            string comandoSql = "SELECT ID, CATEGORIA, DATAINCLUSAO, DATAALTERACAO, STATUS FROM CATEGORIACNH";
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
                            CategoriaCNHDBE categoria = new CategoriaCNHDBE();
                            categoria.ID = reader.GetInt32(0);
                            categoria.Categoria = reader.GetString(1);
                            categoria.DataInclusao = reader.GetDateTime(2);
                            if (!reader.IsDBNull(3))
                                categoria.DataAlteracao = reader.GetDateTime(3);
                            categoria.Status = Convert.ToBoolean(reader.GetInt32(4));
                            lista.Add(categoria);
                        }
                    } 
                }
            }
            listaOrdenada = lista.OrderBy(m => m.ID);
            return lista;
        }
    }
}