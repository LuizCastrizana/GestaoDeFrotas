using GestaoDeFrotas.Data.DBENTITIES;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;

namespace GestaoDeFrotas.Data.DAL
{
    public class CNHDAL : OracleBaseDAL, IDal<CNHDBE>
    {
        // TODO: Testar todos os métodos
        // TODO: Usar parameters

        public CNHDBE Read(int id)
        {
            var retorno = new CNHDBE();

            string comandoSql = "SELECT "
                + "C.ID, "
                + "C.NUMERO, "
                + "C.RENACH, "
                + "C.ESPELHO, "
                + "C.DATAEMISSAO, "
                + "C.DATAVALIDADE, "
                + "C.DATAINCLUSAO, "
                + "C.DATAALTERACAO, "
                + "C.STATUS, "
                + "C.CATEGORIAID, "
                + "CA.CATEGORIA "
                + "FROM CNH C "
                + "INNER JOIN CATEGORIACNH CA "
                + "ON C.CATEGORIAID = CA.ID "
                + "WHERE C.ID = :ID";
            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(comandoSql, Conexao))
            {
                Conexao.Open();
                Comando.Parameters.Add("ID", id);

                using (var oracle = Comando.ExecuteReader())
                {
                    if (oracle.HasRows)
                    {
                        oracle.Read();
                        retorno.ID = oracle.GetInt32(0);
                        retorno.Numero = oracle.GetString(1);
                        retorno.RENACH = oracle.GetString(2);
                        retorno.Espelho = oracle.GetString(3);
                        retorno.DataEmissao = oracle.GetDateTime(4);
                        retorno.DataValidade = oracle.GetDateTime(5);
                        retorno.DataInclusao = oracle.GetDateTime(6);
                        if (!oracle.IsDBNull(7))
                            retorno.DataAlteracao = oracle.GetDateTime(7);
                        retorno.Status = Convert.ToBoolean(oracle.GetInt32(8));
                        retorno.Categoria.ID = oracle.GetInt32(9);
                        retorno.Categoria.Categoria = oracle.GetString(10);
                    }
                }
            }

            return retorno;
        }
        public void Create(CNHDBE obj)
        {
            var comandoSql = "INSERT INTO CNH C (C.NUMERO, C.RENACH, C.ESPELHO, C.DATAEMISSAO, C.DATAVALIDADE, C.CATEGORIAID) " +
                        "VALUES (:NUMERO, :RENACH, :ESPELHO, :DATAEMISSAO, :DATAVALIDADE, :CATEGORIAID) RETURNING ID INTO :ID";
            int idNovo;
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(comandoSql, Conexao))
                {
                    Conexao.Open();

                    Comando.Parameters.Add("NUMERO", obj.Numero.ToUpper());
                    Comando.Parameters.Add("RENACH", obj.RENACH.ToUpper());
                    Comando.Parameters.Add("ESPELHO", obj.Espelho.ToUpper());
                    Comando.Parameters.Add("DATAEMISSAO", obj.DataEmissao);
                    Comando.Parameters.Add("DATAVALIDADE", obj.DataValidade);
                    Comando.Parameters.Add("CATEGORIAID", obj.Categoria.ID);

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
                        throw new CadastroCNHException("Erro ao icluir CNH!");
                }
                obj.ID = idNovo;
            }
            catch (Exception e)
            {
                throw new CadastroCNHException("Erro ao incluir CNH", e);
            }
        }
        public void Update(CNHDBE obj)
        {
            string comandoSql = "UPDATE CNH C SET "
                + "C.NUMERO = " + "\'" + obj.Numero + "\', "
                + "C.RENACH = " + "\'" + obj.RENACH + "\', "
                + "C.ESPELHO = " + "\'" + obj.Espelho + "\', "
                + "C.DATAEMISSAO = " + "\'" + obj.DataEmissao.ToString("dd/MM/yyyy") + "\', "
                + "C.DATAVALIDADE = " + "\'" + obj.DataValidade.ToString("dd/MM/yyyy") + "\', "
                + "C.CATEGORIAID = "  + obj.Categoria.ID + " "
                + "WHERE C.ID = " + obj.ID;
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(comandoSql, Conexao))
                {
                    Conexao.Open();
                    if (Comando.ExecuteNonQuery() == 0)
                        throw new CadastroCNHException("Erro ao atualizar CNH!");
                }
            }
            catch (Exception e)
            {
                throw new CadastroCNHException("Erro ao atualizar CNH!", e);
            }
        }
        public void Delete(int id)
        {
            string comandoSql = "DELETE FROM CNH C WHERE C.ID = :ID";
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(comandoSql, Conexao))
                {
                    Conexao.Open();
                    Comando.Parameters.Add("ID", id);

                    if (Comando.ExecuteNonQuery() == 0)
                        throw new CadastroCNHException("Erro ao excluir CNH!");
                }
            }
            catch (Exception e)
            {
                throw new CadastroCNHException("Erro ao excluir CNH!", e);
            }
        }
        /// <summary>
        /// Verifica se os dados da CNH já existem no banco de dados. Se existe insere o valor do ID na propriedade do objeto/>
        /// </summary>
        /// <param name="idMotorista">Referencia para uma variavel que armazena o ID do motorista ao qual está associada a CNH caso exista</param>
        /// <returns>true: existe CNH vinculada a um motorista | false: não existe CNH cadastrada | null: existe CNH mas não está associada a nenhum motorista</returns>
        public bool? VerificaSeEstaCadastrado(ref int idMotorista, CNHDBE obj)
        {
            string buscaCNH = "SELECT ID, NUMERO, RENACH, ESPELHO, DATAEMISSAO, DATAVALIDADE FROM CNH WHERE "
                                + "NUMERO = \'" + obj.Numero + "\' OR RENACH = \'" + obj.RENACH + "\' OR ESPELHO = \'" + obj.Espelho + "\'";
            string buscaMotorista = "SELECT ID FROM MOTORISTA WHERE CNHID = ";

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();
                Comando.CommandText = buscaCNH;
                using (var reader = Comando.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        obj.ID = reader.GetInt32(0);
                        buscaMotorista += obj.ID;
                    }
                    else
                    {
                        // não existe CNH cadastrada com os dados passados no objeto
                        return false;
                    }
                }
                // Existe CNH com algum dos dados passados no objeto
                Comando.CommandText = buscaMotorista;
                using (var reader = Comando.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        // existe CNH e está associada a um motorista
                        reader.Read();
                        idMotorista = reader.GetInt32(0);
                        return true;
                    }
                    else
                    {
                        // Existe CNH cadastrada com algum dos dados do objeto, mas não está associada a nenhum motorista
                        return null;
                    }
                }
            }
        }

        public IEnumerable<CNHDBE> Read(CNHDBE obj)
        {
            throw new NotImplementedException();
        }
    }
}