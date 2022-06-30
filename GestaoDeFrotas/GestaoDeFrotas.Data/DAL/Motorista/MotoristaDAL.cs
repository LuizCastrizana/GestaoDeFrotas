using GestaoDeFrotas.Data.DBENTITIES;
using GestaoDeFrotas.Data.Enums;
using GestaoDeFrotas.Shared.FiltroBusca;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestaoDeFrotas.Data.DAL
{
    public class MotoristaDAL : OracleBaseDAL, IDal<MotoristaDBE>
    {

        /// <summary>
        /// Retorna lista de motoristas cadastrados
        /// </summary>
        /// <param name="todos">true - ativos e inativos | false - apenas ativos | null - apenas inativos</param>
        public IEnumerable<MotoristaDBE> List(bool? todos)
        {
            List<MotoristaDBE> Motoristas = new List<MotoristaDBE>();
            StringBuilder textoComando = new StringBuilder(
                                "SELECT MO.ID, " +
                                "MO.PRIMEIRONOME, " +
                                "MO.SOBRENOME, " +
                                "MO.CPF, " +
                                "CN.ID AS CNHID, " +
                                "MO.DATAINCLUSAO, " +
                                "MO.DATAALTERACAO, " +
                                "MO.STATUS, " +
                                "EN.ID AS ENDERECOID, " +
                                "EN.LOGRADOURO, " +
                                "EN.NUMERO, " +
                                "EN.COMPLEMENTO, " +
                                "EN.MUNICIPIOID AS MUNICIPIOID, " +
                                "EN.CEP " +
                                "FROM MOTORISTA MO " +
                                "INNER JOIN " +
                                "ENDERECO EN " +
                                "ON MO.ENDERECOID = EN.ID " +
                                "INNER JOIN " +
                                "CNH CN " +
                                "ON MO.CNHID = CN.ID " +
                                "WHERE (1 = 1)"
                                );

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();

                if (todos == false) { textoComando.Append(" AND MO.STATUS IN ( :STATUS )"); Comando.Parameters.Add("STATUS", (int)ENUMSTATUS.ATIVO); }
                else if (todos == null) { textoComando.Append(" AND MO.STATUS NOT IN ( :STATUS )"); Comando.Parameters.Add("STATUS", (int)ENUMSTATUS.ATIVO); }

                Comando.CommandText = textoComando.ToString();

                using (var DataReader = Comando.ExecuteReader())
                {
                    if (DataReader.HasRows)
                    {
                        while (DataReader.Read())
                        {
                            MotoristaDBE motorista = new MotoristaDBE();
                            motorista.ID = Convert.ToInt32(DataReader["ID"]);
                            motorista.PrimeiroNome = Convert.ToString(DataReader["PRIMEIRONOME"]);
                            motorista.Sobrenome = Convert.ToString(DataReader["SOBRENOME"]);
                            motorista.CPF = Convert.ToString(DataReader["CPF"]);
                            motorista.CNH = new CNHDAL().Read(Convert.ToInt32(DataReader["CNHID"]));
                            motorista.DataInclusao = Convert.ToDateTime(DataReader["DATAINCLUSAO"]);
                            if (DataReader["DATAALTERACAO"] != DBNull.Value)
                                motorista.DataAlteracao = Convert.ToDateTime(DataReader["DATAALTERACAO"]);
                            motorista.Status = Convert.ToBoolean(DataReader["STATUS"]);
                            motorista.Endereco.ID = Convert.ToInt32(DataReader["ENDERECOID"]);
                            motorista.Endereco.Logradouro = Convert.ToString(DataReader["LOGRADOURO"]);
                            if (DataReader["NUMERO"] != DBNull.Value)
                                motorista.Endereco.Numero = Convert.ToInt32(DataReader["NUMERO"]);
                            if (DataReader["COMPLEMENTO"] != DBNull.Value)
                                motorista.Endereco.Complemento = Convert.ToString(DataReader["COMPLEMENTO"]);
                            motorista.Endereco.Municipio.ID = Convert.ToInt32(DataReader["MUNICIPIOID"]);
                            motorista.Endereco.Cep = Convert.ToString(DataReader["CEP"]);
                            motorista.Endereco.Municipio = 
                                new MunicipioDAL().ListarMunicipios().Where(m => m.ID == motorista.Endereco.Municipio.ID).First();
                            Motoristas.Add(motorista);
                        }
                    }
                }
            }
            return Motoristas;
        }
        /// <summary>
        /// Retorna motoristas vinculados a um veículo.
        /// </summary>
        /// <param name="id">ID do veiculo</param>
        /// <param name="todos">true - ativos e inativos | false - apenas ativos | null - apenas inativos</param>
        public IEnumerable<MotoristaDBE> ListByVeiculoID(int id, bool? todos)
        {
            List<MotoristaDBE> Motoristas = new List<MotoristaDBE>();
            StringBuilder textoComando = new StringBuilder(
                                "SELECT M.ID, " +
                                "M.PRIMEIRONOME, " +
                                "M.SOBRENOME, " +
                                "M.CPF, " +
                                "M.STATUS, " +
                                "E.LOGRADOURO, " +
                                "E.NUMERO, " +
                                "E.COMPLEMENTO, " +
                                "MU.NOME, " +
                                "ES.UF, " +
                                "E.CEP " +
                                "FROM MOTORISTA M " +
                                "INNER JOIN MOTORISTA_VEICULO MV " +
                                "ON M.ID = MV.MOTORISTAID " +
                                "INNER JOIN ENDERECO E " +
                                "ON M.ENDERECOID = E.ID " +
                                "INNER JOIN MUNICIPIO MU " +
                                "ON E.MUNICIPIOID = MU.ID " +
                                "INNER JOIN ESTADO ES " +
                                "ON MU.ESTADOID = ES.ID " +
                                "WHERE VEICULOID = :ID"
                                );

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();
                Comando.Parameters.Add("ID", id);

                if (todos == false) { textoComando.Append(" AND M.STATUS IN ( :STATUS )"); Comando.Parameters.Add("STATUS", (int)ENUMSTATUS.ATIVO); }
                else if (todos == null) { textoComando.Append(" AND M.STATUS NOT IN ( :STATUS )"); Comando.Parameters.Add("STATUS", (int)ENUMSTATUS.ATIVO); }

                Comando.CommandText = textoComando.ToString();

                using (var DataReader = Comando.ExecuteReader())
                {
                    if (DataReader.HasRows)
                    {
                        while (DataReader.Read())
                        {
                            MotoristaDBE motorista = new MotoristaDBE();
                            motorista.ID = Convert.ToInt32(DataReader["ID"]);
                            motorista.PrimeiroNome = Convert.ToString(DataReader["PRIMEIRONOME"]);
                            motorista.Sobrenome = Convert.ToString(DataReader["SOBRENOME"]);
                            motorista.CPF = Convert.ToString(DataReader["CPF"]);
                            motorista.Status = Convert.ToBoolean(DataReader["STATUS"]);
                            motorista.Endereco.Logradouro = Convert.ToString(DataReader["LOGRADOURO"]);
                            if (DataReader["NUMERO"] != DBNull.Value)
                                motorista.Endereco.Numero = Convert.ToInt32(DataReader["NUMERO"]);
                            if (DataReader["COMPLEMENTO"] != DBNull.Value)
                                motorista.Endereco.Complemento = Convert.ToString(DataReader["COMPLEMENTO"]);
                            motorista.Endereco.Municipio.NomeMunicipio = Convert.ToString(DataReader["NOME"]);
                            motorista.Endereco.Municipio.Estado.UF = Convert.ToString(DataReader["UF"]);
                            motorista.Endereco.Cep = Convert.ToString(DataReader["CEP"]);
                            Motoristas.Add(motorista);
                        }
                    }
                }
            }
            return Motoristas;
        }

        /// <summary>
        /// Busca dados do motorista por CPF e preenche as propriedades do objeto
        /// </summary>
        /// <param name="cpf">CPF a ser buscado</param>
        /// <param name="status">true - ativos e inativos | false - apenas ativos | null - apenas inativos</param>
        public MotoristaDBE GetByCPF(string cpf, bool? todos)
        {
            var retorno = new MotoristaDBE();

            StringBuilder textoComando = new StringBuilder("SELECT MO.ID, " +
                                "MO.PRIMEIRONOME, " +
                                "MO.SOBRENOME, " +
                                "MO.CPF, " +
                                "MO.RG," +
                                "MO.DATANASCIMENTO," +
                                "MO.DATAINCLUSAO, " +
                                "MO.DATAALTERACAO, " +
                                "MO.STATUS, " +
                                "EN.ID AS ENDERECOID, " +
                                "EN.LOGRADOURO, " +
                                "EN.NUMERO, " +
                                "EN.COMPLEMENTO, " +
                                "EN.BAIRRO, " +
                                "EN.MUNICIPIOID, " +
                                "MN.ESTADOID, " +
                                "EN.CEP, " +
                                "MO.CNHID " +
                                "FROM MOTORISTA MO " +
                                "INNER JOIN " +
                                "ENDERECO EN " +
                                "ON MO.ENDERECOID = EN.ID " +
                                "INNER JOIN " +
                                "MUNICIPIO MN " +
                                "ON EN.MUNICIPIOID = MN.ID " +
                                "WHERE MO.CPF = :CPF");

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();
                Comando.Parameters.Add("CPF", cpf);

                if (todos == false) { textoComando.Append(" AND MO.STATUS IN ( :STATUS )"); Comando.Parameters.Add("STATUS", (int)ENUMSTATUS.ATIVO); }
                else if (todos == null) { textoComando.Append(" AND MO.STATUS NOT IN ( :STATUS )"); Comando.Parameters.Add("STATUS", (int)ENUMSTATUS.ATIVO); }

                Comando.CommandText = textoComando.ToString();
                //try catch
                using (var DataReader = Comando.ExecuteReader())
                {
                    if (DataReader.HasRows)
                    {
                        DataReader.Read();
                        retorno.ID = Convert.ToInt32(DataReader["ID"]);
                        retorno.PrimeiroNome = Convert.ToString(DataReader["PRIMEIRONOME"]);
                        retorno.Sobrenome = Convert.ToString(DataReader["SOBRENOME"]);
                        retorno.CPF = Convert.ToString(DataReader["CPF"]);
                        if (DataReader["RG"] != DBNull.Value)
                            retorno.RG = Convert.ToString(DataReader["RG"]);
                        retorno.DataNascimento = Convert.ToDateTime(DataReader["DATANASCIMENTO"]);
                        retorno.DataInclusao = Convert.ToDateTime(DataReader["DATAINCLUSAO"]);
                        if (DataReader["DATAALTERACAO"] != DBNull.Value)
                            retorno.DataAlteracao = Convert.ToDateTime(DataReader["DATAALTERACAO"]);
                        retorno.Status = Convert.ToBoolean(DataReader["STATUS"]);
                        retorno.Endereco.ID = Convert.ToInt32(DataReader["ENDERECOID"]);
                        retorno.Endereco.Logradouro = Convert.ToString(DataReader["LOGRADOURO"]);
                        if (DataReader["NUMERO"] != DBNull.Value)
                            retorno.Endereco.Numero = Convert.ToInt32(DataReader["NUMERO"]);
                        if (DataReader["COMPLEMENTO"] != DBNull.Value)
                            retorno.Endereco.Complemento = Convert.ToString(DataReader["COMPLEMENTO"]);
                        if (DataReader["BAIRRO"] != DBNull.Value)
                            retorno.Endereco.Bairro = Convert.ToString(DataReader["BAIRRO"]);
                        retorno.Endereco.Municipio.ID = Convert.ToInt32(DataReader["MUNICIPIOID"]);
                        retorno.Endereco.Municipio.Estado.ID = Convert.ToInt32(DataReader["ESTADOID"]);
                        retorno.Endereco.Cep = Convert.ToString(DataReader["CEP"]);
                        retorno.CNH = new CNHDAL().Read(Convert.ToInt32(DataReader["CNHID"]));
                    }
                }
            }

            return retorno;
        }

        /// <summary>
        /// Inclui um novo motorista e endereço no banco de dados com os dados do objeto passado como parâmetro.
        /// Em caso de exception verificar inner exception.
        /// </summary>
        /// <param name="obj">Objeto com os dados a serem atualizados</param>
        /// <exception cref="CadastroMotoristaException">Captura qualquer exceção que ocorreu ao conectar com o banco ou executar o comando e salva na inner exception</exception>
        public void Create(MotoristaDBE obj)
        {
            string comandoSql = "INSERT INTO MOTORISTA M (M.PRIMEIRONOME, M.SOBRENOME, M.CPF, M.RG, M.DATANASCIMENTO, M.CNHID, M.ENDERECOID) " +
                "VALUES (:PRIMEIRONOME, :SOBRENOME, :CPF, :RG, :DATANASCIMENTO, :CNHID, :ENDERECOID)";
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(null, Conexao))
                {
                    Conexao.Open();

                    Comando.CommandText = comandoSql;
                    Comando.Parameters.Add("PRIMEIRONOME", obj.PrimeiroNome);
                    Comando.Parameters.Add("SOBRENOME", obj.Sobrenome);
                    Comando.Parameters.Add("CPF", obj.CPF);
                    Comando.Parameters.Add("RG", obj.RG);
                    Comando.Parameters.Add("DATANASCIMENTO", obj.DataNascimento.ToString("dd/MM/yyyy"));
                    Comando.Parameters.Add("CNHID", obj.CNH.ID);
                    Comando.Parameters.Add("ENDERECOID", obj.Endereco.ID);

                    if (Comando.ExecuteNonQuery() == 0)
                    {
                        throw new CadastroMotoristaException("Erro ao cadastrar motorista!");
                    }
                }
            }
            catch (Exception e)
            {
                throw new CadastroMotoristaException("Erro ao cadastrar motorista!", e);
            }
        }

        /// <summary>
        /// Atualiza dados do motorista com os dados do objeto passado como parâmetro.
        /// Em caso de exception verificar inner exception.
        /// </summary>
        /// <param name="obj">Objeto com os dados a serem atualizados</param>
        /// <exception cref="CadastroMotoristaException"></exception>
        public void Update(MotoristaDBE obj)
        {
            string comandoSql = "UPDATE MOTORISTA M SET M.PRIMEIRONOME = :PRIMEIRONOME, M.SOBRENOME = :SOBRENOME, M.DATANASCIMENTO = :DATANASCIMENTO WHERE M.ID = :ID";
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(null, Conexao))
                {
                    Conexao.Open();

                    Comando.CommandText = comandoSql;
                    Comando.Parameters.Add("PRIMEIRONOME", obj.PrimeiroNome);
                    Comando.Parameters.Add("SOBRENOME", obj.Sobrenome);
                    Comando.Parameters.Add("DATANASCIMENTO", obj.DataNascimento.ToString("dd/MM/yyyy"));
                    Comando.Parameters.Add("ID", obj.ID);

                    if (Comando.ExecuteNonQuery() == 0)
                    {
                        throw new CadastroMotoristaException("Erro ao editar motorista!");
                    }
                }
            }
            catch (Exception e)
            {
                throw new CadastroMotoristaException("Erro ao editar motorista!", e);
            }
        }

        /// <summary>
        /// Atualiza status do motorista
        /// </summary>
        /// <param name="id">ID do motorista a ser atualizado</param>
        /// <param name="status">Status a inserir</param>
        /// <exception cref="CadastroMotoristaException"></exception>
        public void AtualizarStatus(int id, bool status)
        {
            StringBuilder textoComando = new StringBuilder("UPDATE MOTORISTA SET STATUS = :STATUS WHERE ID = :ID"); 
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(null, Conexao))
                {
                    Conexao.Open();
                    Comando.CommandText = textoComando.ToString();
                    Comando.Parameters.Add("STATUS", Convert.ToInt32(status));
                    Comando.Parameters.Add("ID", id);
                    if (Comando.ExecuteNonQuery() == 0)
                    {
                        throw new CadastroMotoristaException("Erro ao atualizar status do motorista!");
                    }
                }
            }
            catch (Exception e)
            {
                throw new CadastroMotoristaException("Erro ao atualizar status do motorista!", e);
            }
        }

        /// <summary>
        /// Vincula um veículo ao motorista
        /// </summary>
        /// <param name="idVeiculo">ID do veículo</param>
        /// <param name="idMotorista">ID do motorista</param>
        public void VincularVeiculoMotorista(int idVeiculo, int idMotorista)
        {
            string comandoSql = "INSERT INTO MOTORISTA_VEICULO MV " +
                "(MV.MOTORISTAID, " +
                "MV.VEICULOID) " +
                "VALUES " +
                "(" +
                idMotorista.ToString() + ", " +
                idVeiculo.ToString() +
                ")";
            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();
                Comando.CommandText = comandoSql;
                if (Comando.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Erro ao vincular veículo!");
                }
            }
        }

        /// <summary>
        /// Remove vinculo entre um motorista e um veículo
        /// </summary>
        /// <param name="veiculoid">ID do veículo</param>
        /// <param name="motoristaid">ID do motorista</param>
        public void DesvincularVeiculoMotorista(int veiculoid, int motoristaid)
        {
            string comandoSql = "DELETE FROM MOTORISTA_VEICULO MV " +
                "WHERE MV.VEICULOID = " +
                veiculoid.ToString() +
                "AND MV.MOTORISTAID = " +
                motoristaid.ToString();
            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();
                Comando.CommandText = comandoSql;
                if (Comando.ExecuteNonQuery() == 0)
                {
                    throw new CadastroMotoristaException("Erro ao desvincular veículo!");
                }
            }
        }

        /// <summary>
        /// Busca motorista por ID
        /// </summary>
        /// <param name="id">ID do motorista</param>
        /// <returns></returns>
        public MotoristaDBE Read(int id)
        {
            var retorno = new MotoristaDBE();

            StringBuilder textoComando = new StringBuilder("SELECT MO.ID, " +
                                "MO.PRIMEIRONOME, " +
                                "MO.SOBRENOME, " +
                                "MO.CPF, " +
                                "MO.RG," +
                                "MO.DATANASCIMENTO," +
                                "MO.DATAINCLUSAO, " +
                                "MO.DATAALTERACAO, " +
                                "MO.STATUS, " +
                                "MO.ENDERECOID, " +
                                "MO.CNHID " +
                                "FROM MOTORISTA MO " +
                                "WHERE MO.ID = :ID");

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(textoComando.ToString(), Conexao))
            {
                Conexao.Open();
                Comando.Parameters.Add("ID", id);

                //try catch
                using (var DataReader = Comando.ExecuteReader())
                {
                    if (DataReader.HasRows)
                    {
                        DataReader.Read();
                        retorno.ID = Convert.ToInt32(DataReader["ID"]);
                        retorno.PrimeiroNome = Convert.ToString(DataReader["PRIMEIRONOME"]);
                        retorno.Sobrenome = Convert.ToString(DataReader["SOBRENOME"]);
                        retorno.CPF = Convert.ToString(DataReader["CPF"]);
                        if (DataReader["RG"] != DBNull.Value)
                            retorno.RG = Convert.ToString(DataReader["RG"]);
                        retorno.DataNascimento = Convert.ToDateTime(DataReader["DATANASCIMENTO"]);
                        retorno.DataInclusao = Convert.ToDateTime(DataReader["DATAINCLUSAO"]);
                        if (DataReader["DATAALTERACAO"] != DBNull.Value)
                            retorno.DataAlteracao = Convert.ToDateTime(DataReader["DATAALTERACAO"]);
                        retorno.Status = Convert.ToBoolean(DataReader["STATUS"]);
                        retorno.Endereco = new EnderecoDAL().Read(Convert.ToInt32(DataReader["ENDERECOID"]));
                        retorno.CNH = new CNHDAL().Read(Convert.ToInt32(DataReader["CNHID"]));
                    }
                    retorno.ListaVeiculos = new VeiculoDAL().ListarVeiculosPorIDMotorista(retorno.ID, true);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Excluir registro de motorista
        /// </summary>
        /// <param name="id">ID do motorista</param>
        public void Delete(int id)
        {
            string comandoSql = "DELETE FROM MOTORISTA WHERE ID = :ID";
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(comandoSql, Conexao))
                {
                    Conexao.Open();
                    Comando.Parameters.Add("ID", id);

                    if (Comando.ExecuteNonQuery() < 1)
                        throw new CadastroMotoristaException("Erro ao excluir motorista!");
                }
            }
            catch (Exception e)
            {
                throw new CadastroMotoristaException("Erro ao excluir motorista", e.InnerException);
            }
        }

        /// <summary>
        /// Busca motoristas com os dados do objeto passado como parametro
        /// </summary>
        /// <param name="obj">Dados a serem buscados</param>
        /// <returns></returns>
        public IEnumerable<MotoristaDBE> List(FiltroBusca filtro)
        {
            var retorno = new List<MotoristaDBE>();

            var query = new StringBuilder($@" SELECT * FROM (
                            SELECT
                                MO.ID                           AS ID,
                                UPPER(MO.PRIMEIRONOME)          AS PRIMEIRONOME,
                                UPPER(MO.SOBRENOME)             AS SOBRENOME,
                                UPPER(MO.PRIMEIRONOME || ' ' ||         
                                MO.SOBRENOME)                   AS NOMECOMPLETO,
                                MO.CPF                          AS CPF,
                                MO.CNHID                        AS CNHID,
                                MO.DATAINCLUSAO                 AS DATAINCLUSAO,
                                MO.DATAALTERACAO                AS DATAALTERACAO,
                                MO.STATUS                       AS STATUS,
                                MO.ENDERECOID                   AS ENDERECOID,
                                CN.NUMERO                       AS NUMEROCNH,
                                UPPER(MN.NOME)                  AS MUNICIPIO
                            FROM MOTORISTA MO
                            INNER JOIN CNH CN ON MO.CNHID = CN.ID
                            INNER JOIN ENDERECO EN ON MO.ENDERECOID = EN.ID
                            INNER JOIN MUNICIPIO MN ON EN.MUNICIPIOID = MN.ID
                            )
                            WHERE (1 = 1)");

            query.Append(MontarFiltros(filtro));

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(query.ToString(), Conexao))
            {
                Conexao.Open();
                using (var DataReader = Comando.ExecuteReader())
                {
                    if (DataReader.HasRows)
                    {
                        while (DataReader.Read())
                        {
                            MotoristaDBE motorista = new MotoristaDBE();
                            motorista.ID = Convert.ToInt32(DataReader["ID"]);
                            motorista.PrimeiroNome = Convert.ToString(DataReader["PRIMEIRONOME"]);
                            motorista.Sobrenome = Convert.ToString(DataReader["SOBRENOME"]);
                            motorista.CPF = Convert.ToString(DataReader["CPF"]);
                            motorista.CNH = new CNHDAL().Read(Convert.ToInt32(DataReader["CNHID"]));
                            motorista.DataInclusao = Convert.ToDateTime(DataReader["DATAINCLUSAO"]);
                            if (DataReader["DATAALTERACAO"] != DBNull.Value)
                                motorista.DataAlteracao = Convert.ToDateTime(DataReader["DATAALTERACAO"]);
                            motorista.Status = Convert.ToBoolean(DataReader["STATUS"]);
                            motorista.Endereco = new EnderecoDAL().Read(Convert.ToInt32(DataReader["ENDERECOID"]));
                            motorista.CNH.Numero = Convert.ToString(DataReader["NUMEROCNH"]);
                            motorista.Endereco.Municipio.NomeMunicipio = Convert.ToString(DataReader["MUNICIPIO"]);
                            retorno.Add(motorista);
                        }
                    }
                }
            }
        return retorno;
        }
    }
}