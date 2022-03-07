using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CadastroDeCaminhoneiro.Models
{
    // TODO: Usar parameters
    public class Veiculo : OracleModel, IModel
    {
        [DisplayName("ID")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression(@"[A-Z]{3}-[0-9]{1}[A-Z0-9]{1}[0-9]{2}", ErrorMessage = "Valor inválido")]
        [Remote("VeiculoExiste", "Veiculo", AdditionalFields = "ID", ErrorMessage = "A placa inserida já foi cadastrada")]
        [DisplayName("Placa")]
        public string Placa { get; set; }

        public MarcaVeiculo Marca { get; set; }

        public ModeloVeiculo Modelo { get; set; }

        public TipoVeiculo Tipo { get; set; }

        public IEnumerable<Motorista> ListaMotoristas { get; set; }

        [DisplayName("Data de inclusão")]
        public DateTime DataInclusao { get; set; }

        [DisplayName("Data de alteração")]
        public DateTime DataAlteracao { get; set; }

        [DisplayName("Ativo")]
        public bool Status { get; set; }

        public Veiculo()
        {
            Marca = new MarcaVeiculo();
            Modelo = new ModeloVeiculo();
            Tipo = new TipoVeiculo();
            ListaMotoristas = Enumerable.Empty<Motorista>();
        }

        #region Listagem
        public IEnumerable<Veiculo> ListarVeiculos(bool? todos)
        {
            List<Veiculo> Veiculos = new List<Veiculo>();
            string comandoSql = "SELECT V.ID, " +
                                "V.PLACA, " +
                                "V.DATAINCLUSAO, " +
                                "V.DATAALTERACAO, " +
                                "V.STATUS, " +
                                "MA.ID, " +
                                "MA.NOME, " +
                                "MO.ID, " +
                                "MO.NOME, " +
                                "E.ID, " +
                                "E.DESCRICAO " +
                                "FROM VEICULO V " +
                                "INNER JOIN MARCAVEICULO MA " +
                                "ON V.MARCAVEICULOID = MA.ID " +
                                "INNER JOIN MODELOVEICULO MO " +
                                "ON V.MODELOVEICULOID = MO.ID " +
                                "INNER JOIN TIPOVEICULO E " +
                                "ON V.TIPOVEICULOID = E.ID ";
            switch (todos)
            {
                case true:
                    break;
                case false:
                    comandoSql += "WHERE V.STATUS = 1";
                    break;
                case null:
                    comandoSql += "WHERE V.STATUS = 0";
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
                            Veiculo veiculo = new Veiculo();
                            veiculo.ID = reader.GetInt32(0);
                            veiculo.Placa = reader.GetString(1);
                            veiculo.DataInclusao = reader.GetDateTime(2);
                            if (!reader.IsDBNull(3))
                                veiculo.DataAlteracao = reader.GetDateTime(3);
                            veiculo.Status = Convert.ToBoolean(reader.GetInt32(4));
                            veiculo.Marca.ID = reader.GetInt32(5);
                            veiculo.Marca.Nome = reader.GetString(6);
                            veiculo.Modelo.ID = reader.GetInt32(7);
                            veiculo.Modelo.Nome = reader.GetString(8);
                            veiculo.Tipo.ID = reader.GetInt32(9);
                            veiculo.Tipo.Descricao = reader.GetString(10);
                            Veiculos.Add(veiculo);
                        }
                    }
                }
            }

            return Veiculos;
        }
        public IEnumerable<Veiculo> ListarVeiculosPorIDMotorista(int id, bool? status)
        {
            List<Veiculo> Veiculos = new List<Veiculo>();

            string comandoSql = "SELECT V.ID, " +
                                "V.PLACA, " +
                                "V.DATAINCLUSAO, " +
                                "V.DATAALTERACAO, " +
                                "V.STATUS, " +
                                "MA.ID, " +
                                "MA.NOME, " +
                                "MO.ID, " +
                                "MO.NOME, " +
                                "E.ID, " +
                                "E.DESCRICAO " +
                                "FROM VEICULO V " +
                                "INNER JOIN MARCAVEICULO MA " +
                                "ON V.MARCAVEICULOID = MA.ID " +
                                "INNER JOIN MODELOVEICULO MO " +
                                "ON V.MODELOVEICULOID = MO.ID " +
                                "INNER JOIN TIPOVEICULO E " +
                                "ON V.TIPOVEICULOID = E.ID " +
                                "INNER JOIN MOTORISTA_VEICULO MV " +
                                "ON V.ID = MV.VEICULOID " +
                                "WHERE MOTORISTAID = " + id.ToString();
            switch (status)
            {
                case true:
                    comandoSql += " AND V.STATUS = 1";
                    break;
                case false:
                    comandoSql += " AND V.STATUS = 0";
                    break;
                default:
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
                            Veiculo veiculo = new Veiculo();
                            veiculo.ID = reader.GetInt32(0);
                            veiculo.Placa = reader.GetString(1);
                            veiculo.DataInclusao = reader.GetDateTime(2);
                            if (!reader.IsDBNull(3))
                                veiculo.DataAlteracao = reader.GetDateTime(3);
                            veiculo.Status = Convert.ToBoolean(reader.GetInt32(4));
                            veiculo.Marca.ID = reader.GetInt32(5);
                            veiculo.Marca.Nome = reader.GetString(6);
                            veiculo.Modelo.ID = reader.GetInt32(7);
                            veiculo.Modelo.Nome = reader.GetString(8);
                            veiculo.Tipo.ID = reader.GetInt32(9);
                            veiculo.Tipo.Descricao = reader.GetString(10);
                            Veiculos.Add(veiculo);
                        }
                    }
                }
            }

            return Veiculos;
        }
        #endregion

        #region Busca
        public Veiculo BuscarPorPlaca(string placa, bool? status)
        {
            Veiculo veiculo = new Veiculo();

            string comandoSql = "SELECT V.ID, " +
                                "V.PLACA, " +
                                "V.MARCAVEICULOID, " +
                                "V.MODELOVEICULOID, " +
                                "V.TIPOVEICULOID, " +
                                "V.DATAINCLUSAO, " +
                                "V.DATAALTERACAO, " +
                                "V.STATUS " +
                                "FROM VEICULO V " +
                                "WHERE V.PLACA = " + 
                                "\'" + placa + "\'";
            switch (status)
            {
                case true:
                    comandoSql += " AND V.STATUS = 1";
                    break;
                case false:
                    comandoSql += " AND V.STATUS = 0";
                    break;
                default:
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
                        reader.Read();
                        veiculo.ID = reader.GetInt32(0);
                        veiculo.Placa = reader.GetString(1);
                        veiculo.Marca = new MarcaVeiculo().BuscarPorId(reader.GetInt32(2));
                        veiculo.Modelo = new ModeloVeiculo().BuscarPorId(reader.GetInt32(3));
                        veiculo.Tipo = new TipoVeiculo().BuscarPorId(reader.GetInt32(4));
                        veiculo.DataInclusao = reader.GetDateTime(5);
                        if (!reader.IsDBNull(6))
                            veiculo.DataAlteracao = reader.GetDateTime(6);
                        veiculo.Status = Convert.ToBoolean(reader.GetInt32(7));
                    }
                }
            }

            return veiculo;
        }
        public Veiculo BuscarPorId(int id, bool? status)
        {
            Veiculo veiculo = new Veiculo();

            string comandoSql = "SELECT V.ID, " +
                                "V.PLACA, " +
                                "V.MARCAVEICULOID, " +
                                "V.MODELOVEICULOID, " +
                                "V.TIPOVEICULOID, " +
                                "V.DATAINCLUSAO, " +
                                "V.DATAALTERACAO, " +
                                "V.STATUS " +
                                "FROM VEICULO V " +
                                "WHERE V.ID = " + id.ToString();
            switch (status)
            {
                case true:
                    comandoSql += " AND V.STATUS = 1";
                    break;
                case false:
                    comandoSql += " AND V.STATUS = 0";
                    break;
                default:
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
                        reader.Read();
                        veiculo.ID = reader.GetInt32(0);
                        veiculo.Placa = reader.GetString(1);
                        veiculo.Marca = new MarcaVeiculo().BuscarPorId(reader.GetInt32(2));
                        veiculo.Modelo = new ModeloVeiculo().BuscarPorId(reader.GetInt32(3));
                        veiculo.Tipo = new TipoVeiculo().BuscarPorId(reader.GetInt32(4));
                        veiculo.DataInclusao = reader.GetDateTime(5);
                        if (!reader.IsDBNull(6))
                            veiculo.DataAlteracao = reader.GetDateTime(6);
                        veiculo.Status = Convert.ToBoolean(reader.GetInt32(7));
                    }
                }
            }

            return veiculo;
        }
        #endregion

        #region Inclusão/Edição
        public void Create()
        {
            string comandoSql = "INSERT INTO VEICULO V " +
                "(V.PLACA, " +
                "V.MARCAVEICULOID, " +
                "V.MODELOVEICULOID, " +
                "V.TIPOVEICULOID) " +
                "VALUES " +
                "(" +
                "\'" + Placa.ToUpper() + "\', " +
                Marca.ID.ToString() + "," +
                Modelo.ID.ToString() + "," +
                Tipo.ID.ToString() +
                ")";

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(comandoSql, Conexao))
            {
                Conexao.Open();
                if (Comando.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Erro ao incluir veículo. Os dados não foram inseridos no banco de dados.");
                }
            }
        }
        public void Update()
        {
            string comandoSql = "UPDATE VEICULO V " +
                "SET V.MARCAVEICULOID = " + Marca.ID + 
                ", V.MODELOVEICULOID =  "  + Modelo.ID +
                ", V.TIPOVEICULOID =  " + Tipo.ID +
                " WHERE V.ID = " + ID.ToString();

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(comandoSql, Conexao))
            {
                Conexao.Open();
                if (Comando.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Erro ao editar veículo. Os dados não foram inseridos no banco de dados.");
                }
            }
        }
        public void DesativarVeiculo(int id)
        {
            string comandoSql = "UPDATE VEICULO V " +
                "SET V.STATUS = " + 0 +
                " WHERE V.ID = " + id.ToString();

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(comandoSql, Conexao))
            {
                Conexao.Open();
                if (Comando.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Erro ao desativar veículo.");
                }
            }
        }

        public void ReativarVeiculo(int id)
        {
            string comandoSql = "UPDATE VEICULO V " +
                "SET V.STATUS = " + 1 +
                " WHERE V.ID = " + id.ToString();

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(comandoSql, Conexao))
            {
                Conexao.Open();
                if (Comando.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Erro ao reativar veículo.");
                }
            }
        }
        #endregion

        public void Read(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(int id, bool status)
        {
            throw new NotImplementedException();
        }
    }
}
