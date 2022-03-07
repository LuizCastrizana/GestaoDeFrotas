using GestaoDeFrotas.Data.DBENTITIES;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoDeFrotas.Data.DAL
{
    // TODO: Usar parameters
    public class VeiculoDAL : OracleBaseDAL, IDal<VeiculoDBE>
    {
        #region Listagem
        public IEnumerable<VeiculoDBE> ListarVeiculos(bool? todos)
        {
            List<VeiculoDBE> Veiculos = new List<VeiculoDBE>();
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
                            VeiculoDBE veiculo = new VeiculoDBE();
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
        public IEnumerable<VeiculoDBE> ListarVeiculosPorIDMotorista(int id, bool? status)
        {
            List<VeiculoDBE> Veiculos = new List<VeiculoDBE>();

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
                            VeiculoDBE veiculo = new VeiculoDBE();
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
        public VeiculoDBE BuscarPorPlaca(string placa, bool? status)
        {
            VeiculoDBE veiculo = new VeiculoDBE();

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
                        veiculo.Marca = new MarcaVeiculoDAL().BuscarPorId(reader.GetInt32(2));
                        veiculo.Modelo = new ModeloVeiculoDAL().BuscarPorId(reader.GetInt32(3));
                        veiculo.Tipo = new TipoVeiculoDAL().BuscarPorId(reader.GetInt32(4));
                        veiculo.DataInclusao = reader.GetDateTime(5);
                        if (!reader.IsDBNull(6))
                            veiculo.DataAlteracao = reader.GetDateTime(6);
                        veiculo.Status = Convert.ToBoolean(reader.GetInt32(7));
                    }
                }
            }

            return veiculo;
        }
        public VeiculoDBE BuscarPorId(int id, bool? status)
        {
            VeiculoDBE veiculo = new VeiculoDBE();

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
                        veiculo.Marca = new MarcaVeiculoDAL().BuscarPorId(reader.GetInt32(2));
                        veiculo.Modelo = new ModeloVeiculoDAL().BuscarPorId(reader.GetInt32(3));
                        veiculo.Tipo = new TipoVeiculoDAL().BuscarPorId(reader.GetInt32(4));
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
        public void Create(VeiculoDBE obj)
        {
            string comandoSql = "INSERT INTO VEICULO V " +
                "(V.PLACA, " +
                "V.MARCAVEICULOID, " +
                "V.MODELOVEICULOID, " +
                "V.TIPOVEICULOID) " +
                "VALUES " +
                "(" +
                "\'" + obj.Placa.ToUpper() + "\', " +
                obj.Marca.ID.ToString() + "," +
                obj.Modelo.ID.ToString() + "," +
                obj.Tipo.ID.ToString() +
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
        public void Update(VeiculoDBE obj)
        {
            string comandoSql = "UPDATE VEICULO V " +
                "SET V.MARCAVEICULOID = " + obj.Marca.ID + 
                ", V.MODELOVEICULOID =  "  + obj.Modelo.ID +
                ", V.TIPOVEICULOID =  " + obj.Tipo.ID +
                " WHERE V.ID = " + obj.ID.ToString();

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

        public VeiculoDBE Read(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(int id, bool status)
        {
            throw new NotImplementedException();
        }
    }
}
