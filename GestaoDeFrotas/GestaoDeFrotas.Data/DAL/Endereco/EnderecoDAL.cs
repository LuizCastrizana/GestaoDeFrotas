using GestaoDeFrotas.Data.DBENTITIES;
using GestaoDeFrotas.Shared.FiltroBusca;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace GestaoDeFrotas.Data.DAL
{
    public class EnderecoDAL : OracleBaseDAL, IDal<EnderecoDBE>
    {
        /// <summary>
        /// Insere endereço no banco de dados com os dados do objeto passado
        /// </summary>
        /// <param name="obj">Objeto contendo os dados a serem inseridos</param>
        public void Create(EnderecoDBE obj)
        {
            string comandoSql = "INSERT INTO ENDERECO E (E.LOGRADOURO, E.NUMERO, E.COMPLEMENTO, E.BAIRRO, E.MUNICIPIOID, E.CEP) " +
                                "VALUES (:LOGRADOURO, :NUMERO, :COMPLEMENTO, :BAIRRO, :MUNICIPIOID, :CEP) RETURNING ID INTO :ID";
            int idNovo;
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(null, Conexao))
                {
                    Conexao.Open();

                    Comando.Parameters.Add("LOGRADOURO", obj.Logradouro.ToUpper());
                    if (obj.Numero.HasValue) Comando.Parameters.Add("NUMERO", obj.Numero);
                    else Comando.Parameters.Add("NUMERO", null);
                    if (obj.Complemento != null) Comando.Parameters.Add("COMPLEMENTO", obj.Complemento.ToUpper());
                    else Comando.Parameters.Add("COMPLEMENTO", null);
                    if (obj.Bairro != null) Comando.Parameters.Add("BAIRRO", obj.Bairro.ToUpper());
                    else Comando.Parameters.Add("BAIRRO", null);
                    Comando.Parameters.Add("MUNICIPIOID", obj.Municipio.ID);
                    Comando.Parameters.Add("CEP", obj.Cep);

                    Comando.Parameters.Add(new OracleParameter
                    {
                        ParameterName = "ID",
                        OracleDbType = OracleDbType.Decimal,
                        Direction = ParameterDirection.Output
                    });

                    Comando.CommandText = comandoSql;
                    if (Comando.ExecuteNonQuery() == 1)
                        idNovo = Convert.ToInt32(Comando.Parameters["ID"].Value.ToString());
                    else
                        throw new CadastroEnderecoException("Erro ao icluir endereço!");
                } 
                obj.ID = idNovo;
            }
            catch (Exception e)
            {
                throw new CadastroEnderecoException("Erro ao incluir endereço", e.InnerException);
            }
        }
        /// <summary>
        /// Exclui endereço
        /// </summary>
        /// <param name="id">ID do endereço a ser excluido</param>
        public void Delete(int id)
        {
            string comandoSql = "DELETE FROM ENDERECO WHERE ID = :ID";
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(comandoSql, Conexao))
                {
                    Conexao.Open();
                    Comando.Parameters.Add("ID", id);

                    if (Comando.ExecuteNonQuery() < 1)
                        throw new CadastroEnderecoException("Erro ao excluir endereço!");
                }
            }
            catch (Exception e)
            {
                throw new CadastroEnderecoException("Erro ao excluir endereço", e.InnerException, Convert.ToUInt32(id));
            }
        }
        /// <summary>
        /// Atualiza o endereço no banco de dados com os dados do objeto passado
        /// </summary>
        /// <param name="obj">Objeto contendo os dados a serem inseridos</param>
        public void Update(EnderecoDBE obj)
        {
            string comandoSql = "UPDATE ENDERECO E " +
                    "SET E.LOGRADOURO = " + "\'" + obj.Logradouro.ToUpper() + "\'";
            comandoSql += obj.Numero is null ? ", E.NUMERO = NULL " : ", E.NUMERO = " + obj.Numero;
            comandoSql += obj.Complemento is null ? ", E.COMPLEMENTO = \'\' " : ", E.COMPLEMENTO = \'" + obj.Complemento.ToUpper() + "\'";
            comandoSql += obj.Bairro is null ? ", E.BAIRRO = \'\' " : ", E.BAIRRO = \'" + obj.Bairro.ToUpper() + "\'";
            comandoSql +=
                ", E.CEP =  " + "\'" + obj.Cep.ToUpper() + "\'" +
                ", E.MUNICIPIOID = " + obj.Municipio.ID +
                " WHERE E.ID = " + obj.ID;
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(comandoSql, Conexao))
                {
                    Conexao.Open();
                    if (Comando.ExecuteNonQuery() < 1)
                        throw new CadastroEnderecoException("Erro ao editar endereço!");
                }
            }
            catch (Exception e)
            {
                throw new CadastroEnderecoException("Erro ao editar endereço!", e);
            }
        }

        /// <summary>
        /// Busca endereço por ID
        /// </summary>
        /// <param name="id">ID do endereço</param>
        /// <returns></returns>
        public EnderecoDBE Read(int id)
        {
            StringBuilder comandoSQL = new StringBuilder("SELECT ID, LOGRADOURO, NUMERO, COMPLEMENTO, BAIRRO, MUNICIPIOID, CEP, DATAINCLUSAO, DATAALTERACAO, STATUS FROM ENDERECO WHERE ID = :ID");
            var retorno = new EnderecoDBE();

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(comandoSQL.ToString(), Conexao))
            {
                Conexao.Open();
                Comando.Parameters.Add("ID", id);

                using (var reader = Comando.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        retorno.ID = Convert.ToInt32(reader["ID"]);
                        retorno.Logradouro = Convert.ToString(reader["LOGRADOURO"]);
                        if (reader["NUMERO"] != DBNull.Value)
                            retorno.Numero = Convert.ToInt32(reader["NUMERO"]);
                        if (reader["COMPLEMENTO"] != DBNull.Value)
                            retorno.Complemento = Convert.ToString(reader["COMPLEMENTO"]);
                        if (reader["BAIRRO"] != DBNull.Value)
                            retorno.Bairro = Convert.ToString(reader["BAIRRO"]);
                        retorno.Municipio = new MunicipioDAL().Read(Convert.ToInt32(reader["MUNICIPIOID"]));
                        retorno.Cep = Convert.ToString(reader["CEP"]);
                        retorno.DataInclusao = Convert.ToDateTime(reader["DATAINCLUSAO"]);
                        if (reader["DATAALTERACAO"] != DBNull.Value)
                            retorno.DataAlteracao = Convert.ToDateTime(reader["DATAALTERACAO"]);
                        retorno.Status = Convert.ToBoolean(reader["STATUS"]);
                    }
                }
            }

            return retorno;
        }

        public IEnumerable<EnderecoDBE> List(FiltroBusca Filtro)
        {
            throw new NotImplementedException();
        }
    }
}