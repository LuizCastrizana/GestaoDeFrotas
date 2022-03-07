using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using CadastroDeCaminhoneiro.DBEnums;
using System.Text;

namespace CadastroDeCaminhoneiro.Models
{
    public class Motorista : OracleModel, IModel
    {
        [DisplayName("ID")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(50)]
        [RegularExpression(@"[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ ]+", ErrorMessage = "Utilize apenas letras")]
        [DisplayName("Nome")]
        public string PrimeiroNome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(50)]
        [RegularExpression(@"[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ ]+", ErrorMessage = "Utilize apenas letras")]
        [DisplayName("Sobrenome")]
        public string Sobrenome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Data de nascimento")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}\-\d{2}$", ErrorMessage = "CPF inválido")]
        [Remote("ValidaCpf", "Motorista", AdditionalFields = "ID")]
        [DisplayName("CPF")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("RG")]
        public string RG  { get; set; }

        public CNH CNH { get; set; }

        public IEnumerable<Veiculo> ListaVeiculos { get; set;}

        public Endereco Endereco { get; set; }

        [DisplayName("Data de inclusão")]
        public DateTime DataInclusao { get; set; }

        [DisplayName("Data de alteração")]
        public DateTime DataAlteracao { get; set; }

        [DisplayName("Ativo")]
        public bool Status { get; set; }

        public Motorista()
        {
            Endereco = new Endereco();
            CNH = new CNH();
            ListaVeiculos = Enumerable.Empty<Veiculo>();
        }
        /// <summary>
        /// Retorna lista de motoristas cadastrados
        /// </summary>
        /// <param name="todos">true - ativos e inativos | false - apenas ativos | null - apenas inativos</param>
        public IEnumerable<Motorista> List(bool? todos)
        {
            List<Motorista> Motoristas = new List<Motorista>();
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
                            Motorista motorista = new Motorista();
                            motorista.ID = Convert.ToInt32(DataReader["ID"]);
                            motorista.PrimeiroNome = Convert.ToString(DataReader["PRIMEIRONOME"]);
                            motorista.Sobrenome = Convert.ToString(DataReader["SOBRENOME"]);
                            motorista.CPF = Convert.ToString(DataReader["CPF"]);
                            motorista.CNH.Read(Convert.ToInt32(DataReader["CNHID"]));
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
                                new Municipio().ListarMunicipios().Where(m => m.ID == motorista.Endereco.Municipio.ID).First();
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
        public IEnumerable<Motorista> ListByVeiculoID(int id, bool? todos)
        {
            List<Motorista> Motoristas = new List<Motorista>();
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
                            Motorista motorista = new Motorista();
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
        /// Busca motorista por ID
        /// </summary>
        /// <param name="id">ID a ser buscado</param>
        /// <param name="todos">true - ativos e inativos | false - apenas ativos | null - apenas inativos</param>
        public void GetByID (int id, bool? todos)
        {
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
                                "CN.ID AS CNHID " +
                                "FROM MOTORISTA MO " +
                                "INNER JOIN " +
                                "ENDERECO EN " +
                                "ON MO.ENDERECOID = EN.ID " +
                                "INNER JOIN " +
                                "MUNICIPIO MN " +
                                "ON EN.MUNICIPIOID = MN.ID " +
                                "INNER JOIN " +
                                "CNH CN " +
                                "ON MO.CNHID = CN.ID " +
                                "WHERE MO.ID = :ID");

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();
                Comando.Parameters.Add("ID", id);

                if (todos == false) { textoComando.Append(" AND MO.STATUS IN ( :STATUS )"); Comando.Parameters.Add("STATUS", (int)ENUMSTATUS.ATIVO); }
                else if (todos == null) { textoComando.Append(" AND MO.STATUS NOT IN ( :STATUS )"); Comando.Parameters.Add("STATUS", (int)ENUMSTATUS.ATIVO); }

                Comando.CommandText = textoComando.ToString();
                //try catch
                using (var DataReader = Comando.ExecuteReader())
                {
                    if (DataReader.HasRows)
                    {
                        DataReader.Read();
                        ID = Convert.ToInt32(DataReader["ID"]);
                        PrimeiroNome = Convert.ToString(DataReader["PRIMEIRONOME"]);
                        Sobrenome = Convert.ToString(DataReader["SOBRENOME"]);
                        CPF = Convert.ToString(DataReader["CPF"]);
                        if (DataReader["RG"] != DBNull.Value)
                            RG = Convert.ToString(DataReader["RG"]);
                        DataNascimento = Convert.ToDateTime(DataReader["DATANASCIMENTO"]);
                        DataInclusao = Convert.ToDateTime(DataReader["DATAINCLUSAO"]);
                        if (DataReader["DATAALTERACAO"] != DBNull.Value)
                            DataAlteracao = Convert.ToDateTime(DataReader["DATAALTERACAO"]);
                        Status = Convert.ToBoolean(DataReader["STATUS"]);
                        Endereco.ID = Convert.ToInt32(DataReader["ENDERECOID"]);
                        Endereco.Logradouro = Convert.ToString(DataReader["LOGRADOURO"]);
                        if (DataReader["NUMERO"] != DBNull.Value)
                            Endereco.Numero = Convert.ToInt32(DataReader["NUMERO"]);
                        if (DataReader["COMPLEMENTO"] != DBNull.Value)
                            Endereco.Complemento = Convert.ToString(DataReader["COMPLEMENTO"]);
                        if (DataReader["BAIRRO"] != DBNull.Value)
                            Endereco.Bairro = Convert.ToString(DataReader["BAIRRO"]);
                        Endereco.Municipio.ID = Convert.ToInt32(DataReader["MUNICIPIOID"]);
                        Endereco.Municipio.Estado.ID = Convert.ToInt32(DataReader["ESTADOID"]);
                        Endereco.Cep = Convert.ToString(DataReader["CEP"]);
                        CNH.Read(Convert.ToInt32(DataReader["CNHID"]));
                    }
                }
            }
        }
        /// <summary>
        /// Busca dados do motorista por CPF e preenche as propriedades do objeto
        /// </summary>
        /// <param name="cpf">CPF a ser buscado</param>
        /// <param name="status">true - ativos e inativos | false - apenas ativos | null - apenas inativos</param>
        public void GetByCPF(string cpf, bool? todos)
        {
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
                                "CN.ID AS CNHID " +
                                "FROM MOTORISTA MO " +
                                "INNER JOIN " +
                                "ENDERECO EN " +
                                "ON MO.ENDERECOID = EN.ID " +
                                "INNER JOIN " +
                                "MUNICIPIO MN " +
                                "ON EN.MUNICIPIOID = MN.ID " +
                                "INNER JOIN " +
                                "CNH CN " +
                                "ON MO.CNHID = CN.ID " +
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
                        ID = Convert.ToInt32(DataReader["ID"]);
                        PrimeiroNome = Convert.ToString(DataReader["PRIMEIRONOME"]);
                        Sobrenome = Convert.ToString(DataReader["SOBRENOME"]);
                        CPF = Convert.ToString(DataReader["CPF"]);
                        if (DataReader["RG"] != DBNull.Value)
                            RG = Convert.ToString(DataReader["RG"]);
                        DataNascimento = Convert.ToDateTime(DataReader["DATANASCIMENTO"]);
                        DataInclusao = Convert.ToDateTime(DataReader["DATAINCLUSAO"]);
                        if (DataReader["DATAALTERACAO"] != DBNull.Value)
                            DataAlteracao = Convert.ToDateTime(DataReader["DATAALTERACAO"]);
                        Status = Convert.ToBoolean(DataReader["STATUS"]);
                        Endereco.ID = Convert.ToInt32(DataReader["ENDERECOID"]);
                        Endereco.Logradouro = Convert.ToString(DataReader["LOGRADOURO"]);
                        if (DataReader["NUMERO"] != DBNull.Value)
                            Endereco.Numero = Convert.ToInt32(DataReader["NUMERO"]);
                        if (DataReader["COMPLEMENTO"] != DBNull.Value)
                            Endereco.Complemento = Convert.ToString(DataReader["COMPLEMENTO"]);
                        if (DataReader["BAIRRO"] != DBNull.Value)
                            Endereco.Bairro = Convert.ToString(DataReader["BAIRRO"]);
                        Endereco.Municipio.ID = Convert.ToInt32(DataReader["MUNICIPIOID"]);
                        Endereco.Municipio.Estado.ID = Convert.ToInt32(DataReader["ESTADOID"]);
                        Endereco.Cep = Convert.ToString(DataReader["CEP"]);
                        CNH.Read(Convert.ToInt32(DataReader["CNHID"]));
                    }
                }
            }
        }
        /// <summary>
        /// Inclui um novo motorista e endereço no banco de dados com os parâmetros do objeto.
        /// Em caso de exception verificar inner exception.
        /// </summary>
        /// <exception cref="CadastroMotoristaException">Captura qualquer exceção que ocorreu ao conectar com o banco ou executar o comando e salva na inner exception</exception>
        public void Create()
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
                    Comando.Parameters.Add("PRIMEIRONOME", PrimeiroNome);
                    Comando.Parameters.Add("SOBRENOME", Sobrenome);
                    Comando.Parameters.Add("CPF", CPF);
                    Comando.Parameters.Add("RG", RG);
                    Comando.Parameters.Add("DATANASCIMENTO", DataNascimento.ToString("dd/MM/yyyy"));
                    Comando.Parameters.Add("CNHID", CNH.ID);
                    Comando.Parameters.Add("ENDERECOID", Endereco.ID);

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
        /// Atualiza dados do motorista com os parâmetros do objeto.
        /// Em caso de exception verificar inner exception.
        /// </summary>
        /// <exception cref="CadastroMotoristaException"></exception>
        public void Update()
        {
            string comandoSql = "UPDATE MOTORISTA M SET M.PRIMEIRONOME = :PRIMEIRONOME, M.SOBRENOME = :SOBRENOME, M.DATANASCIMENTO = :DATANASCIMENTO WHERE M.ID = :ID";
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(null, Conexao))
                {
                    Conexao.Open();

                    Comando.CommandText = comandoSql;
                    Comando.Parameters.Add("PRIMEIRONOME", PrimeiroNome);
                    Comando.Parameters.Add("SOBRENOME", Sobrenome);
                    Comando.Parameters.Add("DATANASCIMENTO", DataNascimento.ToString("dd/MM/yyyy"));
                    Comando.Parameters.Add("ID", ID);

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
        public void UpdateStatus(int id, bool status)
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
        /// <param name="id">ID do veículo</param>
        public void VincularVeiculo(int id)
        {
            string comandoSql = "INSERT INTO MOTORISTA_VEICULO MV " +
                "(MV.MOTORISTAID, " +
                "MV.VEICULOID) " +
                "VALUES " +
                "(" +
                this.ID.ToString() + ", " +
                id.ToString() +
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

        public void Read(int id)
        {
            throw new NotImplementedException();
        }
    }
}