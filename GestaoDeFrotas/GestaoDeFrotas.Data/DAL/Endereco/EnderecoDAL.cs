using GestaoDeFrotas.Data.DBENTITIES;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

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
            string comandoSql = "DELETE FROM ENDERECO WHERE ID = " + id.ToString(); ;
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(comandoSql, Conexao))
                {
                    Conexao.Open();
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

        public EnderecoDBE Read(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(int id, bool status)
        {
            throw new NotImplementedException();
        }
    }
}