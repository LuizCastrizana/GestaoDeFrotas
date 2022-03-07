using CadastroDeCaminhoneiro.DBEnums;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

namespace CadastroDeCaminhoneiro.Models
{
    public class Viagem : OracleModel, IModel
    {
        public int ID { get; set; }

        [DisplayName("Código da viagem")]
        public string Codigo { get; set; }

        public Motorista MotoristaViagem { get; set; }

        public Veiculo VeiculoViagem { get; set; }

        [DisplayName("Início")]
        public DateTime Inicio { get; set; }

        [DisplayName("Fim")]
        public DateTime Fim { get; set; }

        public MotivoViagem Motivo{ get; set; }

        public StatusViagem ViagemStatus{ get; set; }

        [DisplayName("Data de inclusão")]
        public DateTime DataInclusao { get; set; }

        [DisplayName("Data de alteração")]
        public DateTime DataAlteracao { get; set; }

        [DisplayName("Status")]
        public bool Status { get; set; }

        public Viagem()
        {
            Motivo = new MotivoViagem();
            ViagemStatus = new StatusViagem();
            MotoristaViagem = new Motorista();
            VeiculoViagem = new Veiculo();
        }

        public void Create()
        {
            StringBuilder textoComando = new StringBuilder("INSERT INTO VIAGEM (CODIGO, MOTORISTAID, VEICULOID, INICIO, FIM, MOTIVOID, STATUSVIAGEMID) " +
                "VALUES (:CODIGO, :MOTORISTAID, :VEICULOID, :INICIO, :FIM, :MOTIVOID, :STATUSVIAGEMID)");

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();
                Comando.CommandText = textoComando.ToString();
                Comando.Parameters.Add("CODIGO", Codigo);
                Comando.Parameters.Add("MOTORISTAID", MotoristaViagem.ID);
                Comando.Parameters.Add("VEICULOID", VeiculoViagem.ID);
                Comando.Parameters.Add("INICIO", Inicio);
                if (Fim.Date == DateTime.MinValue.Date) Comando.Parameters.Add("FIM", null);
                else Comando.Parameters.Add("FIM", Fim);
                Comando.Parameters.Add("MOTIVOID", Motivo.ID);
                Comando.Parameters.Add("STATUSVIAGEMID", ViagemStatus.ID);
                if (Comando.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Erro ao cadastrar viagem!");
                }
            }
        }

        public void Read(int id)
        {
            StringBuilder textoComando = new StringBuilder(
                "SELECT ID, CODIGO, MOTORISTAID, VEICULOID, INICIO, FIM, MOTIVOID, STATUSVIAGEMID, DATAINCLUSAO, DATAALTERACAO, STATUS FROM VIAGEM WHERE ID = :ID"
                );

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(textoComando.ToString(), Conexao))
            {
                Conexao.Open();

                Comando.Parameters.Add("ID", id);

                using (var DataReader = Comando.ExecuteReader())
                {
                    if (DataReader.HasRows)
                    {
                        DataReader.Read();
                        ID = Convert.ToInt32(DataReader["ID"]);
                        Codigo = Convert.ToString(DataReader["CODIGO"]);
                        MotoristaViagem.GetByID(Convert.ToInt32(DataReader["MOTORISTAID"]), true);
                        VeiculoViagem = VeiculoViagem.BuscarPorId(Convert.ToInt32(DataReader["VEICULOID"]), null);
                        Inicio = Convert.ToDateTime(DataReader["INICIO"]);
                        if (DataReader["FIM"] != DBNull.Value)
                            Fim = Convert.ToDateTime(DataReader["FIM"]);
                        Motivo.Read(Convert.ToInt32(DataReader["MOTIVOID"]));
                        ViagemStatus.Read(Convert.ToInt32(DataReader["STATUSVIAGEMID"]));
                        DataInclusao = Convert.ToDateTime(DataReader["DATAINCLUSAO"]);
                        if (DataReader["DATAALTERACAO"] != DBNull.Value)
                            DataAlteracao = Convert.ToDateTime(DataReader["DATAALTERACAO"]);
                        Status = Convert.ToBoolean(DataReader["STATUS"]);
                    }
                }
            }
        }
        // TODO: testar
        public IEnumerable<Viagem> Read()
        {
            var lista = new List<Viagem>();

            StringBuilder textoComando = new StringBuilder(
                "SELECT ID, CODIGO, MOTORISTAID, VEICULOID, INICIO, FIM, MOTIVOID, STATUSVIAGEMID, DATAINCLUSAO, DATAALTERACAO, STATUS FROM VIAGEM WHERE (STATUS = 1)");

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                if (!string.IsNullOrEmpty(Codigo)) { textoComando.Append(" AND CODIGO = :CODIGO"); Comando.Parameters.Add("CODIGO", Codigo.ToUpper()); }
                if (MotoristaViagem.ID > 0) { textoComando.Append(" AND MOTORISTAID = :MOTORISTAID"); Comando.Parameters.Add("MOTORISTAID", MotoristaViagem.ID); }
                if (VeiculoViagem.ID > 0) { textoComando.Append(" AND VEICULOID = :VEICULOID"); Comando.Parameters.Add("VEICULOID", VeiculoViagem.ID); }
                if (Inicio.Date != DateTime.MinValue.Date) { textoComando.Append(" AND INICIO = :INICIO"); Comando.Parameters.Add("INICIO", Inicio); }
                if (Fim.Date != DateTime.MinValue.Date) { textoComando.Append(" AND FIM = :FIM"); Comando.Parameters.Add("FIM", Fim); }
                if (Motivo.ID > 0) { textoComando.Append(" AND MOTIVOID = :MOTIVOID"); Comando.Parameters.Add("MOTIVOID", Motivo.ID); }
                if (ViagemStatus.ID > 0) { textoComando.Append(" AND STATUSVIAGEMID = :STATUSVIAGEMID"); Comando.Parameters.Add("STATUSVIAGEMID", ViagemStatus.ID); }
                if (DataInclusao.Date != DateTime.MinValue.Date) { textoComando.Append(" AND DATAINCLUSAO = :DATAINCLUSAO"); Comando.Parameters.Add("DATAINCLUSAO", DataInclusao); }

                Comando.CommandText = textoComando.ToString();

                Conexao.Open();

                using (var reader = Comando.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            Viagem itemLista = new Viagem();
                            itemLista.ID = Convert.ToInt32(reader["ID"]);
                            itemLista.Codigo = Convert.ToString(reader["CODIGO"]);
                            itemLista.MotoristaViagem.GetByID(Convert.ToInt32(reader["MOTORISTAID"]), true);
                            itemLista.VeiculoViagem = VeiculoViagem.BuscarPorId(Convert.ToInt32(reader["VEICULOID"]), null);
                            itemLista.Inicio = Convert.ToDateTime(reader["INICIO"]);
                            if (reader["FIM"] != DBNull.Value)
                                itemLista.Fim = Convert.ToDateTime(reader["FIM"]);
                            itemLista.Motivo.Read(Convert.ToInt32(reader["MOTIVOID"]));
                            itemLista.ViagemStatus.Read(Convert.ToInt32(reader["STATUSVIAGEMID"]));
                            itemLista.DataInclusao = Convert.ToDateTime(reader["DATAINCLUSAO"]);
                            if (reader["DATAALTERACAO"] != DBNull.Value)
                                itemLista.DataAlteracao = Convert.ToDateTime(reader["DATAALTERACAO"]);
                            itemLista.Status = Convert.ToBoolean(reader["STATUS"]);
                            lista.Add(itemLista);
                        }
                    }
                }
            }
            return lista;
        }
        // TODO: testar
        public void Update()
        {
            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();

                StringBuilder textoComando = new StringBuilder(
                "UPDATE VIAGEM SET ");

                if(string.IsNullOrEmpty(Codigo) 
                    && MotoristaViagem.ID <= 0 
                    && VeiculoViagem.ID <= 0 
                    && Inicio.Date == DateTime.MinValue.Date
                    && Fim.Date == DateTime.MinValue.Date
                    && Motivo.ID <= 0 
                    && ViagemStatus.ID <= 0)
                {
                    throw new Exception("Nenhum parâmetro válido encontrado.");
                }

                if (ID <= 0)
                    throw new Exception("ID inválido.");

                if (!string.IsNullOrEmpty(Codigo)) { textoComando.Append("CODIGO = :CODIGO, "); Comando.Parameters.Add("CODIGO", Codigo.ToUpper()); }
                if (MotoristaViagem.ID > 0) { textoComando.Append("MOTORISTAID = :MOTORISTAID, "); Comando.Parameters.Add("MOTORISTAID", MotoristaViagem.ID); }
                if (VeiculoViagem.ID > 0) { textoComando.Append("VEICULOID = :VEICULOID, "); Comando.Parameters.Add("VEICULOID", VeiculoViagem.ID); }
                if (Inicio.Date != DateTime.MinValue.Date) { textoComando.Append("INICIO = :INICIO, "); Comando.Parameters.Add("INICIO", Inicio); }
                if (Fim.Date != DateTime.MinValue.Date) { textoComando.Append("FIM = :FIM, "); Comando.Parameters.Add("FIM", Fim); }
                if (Motivo.ID > 0) { textoComando.Append("MOTIVOID = :MOTIVOID, "); Comando.Parameters.Add("MOTIVOID", Motivo.ID); }
                if (ViagemStatus.ID > 0) { textoComando.Append("STATUSVIAGEMID = :STATUSVIAGEMID, "); Comando.Parameters.Add("STATUSVIAGEMID", ViagemStatus.ID); }

                textoComando.Remove(textoComando.Length - 2, 2);
                textoComando.Append(" WHERE ID = :ID");
                Comando.Parameters.Add("ID", ID);

                Comando.CommandText = textoComando.ToString();

                if (Comando.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Erro ao atualizar viagem!");
                }
            }
        }

        public void UpdateStatus(int id, bool status)
        {
            StringBuilder textoComando = new StringBuilder("UPDATE VIAGEM SET STATUS = :STATUS WHERE ID = :ID");

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(textoComando.ToString(), Conexao))
            {
                Conexao.Open();

                Comando.Parameters.Add("STATUS", Convert.ToInt32(status));
                Comando.Parameters.Add("ID", id);

                if (Comando.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Erro ao atualizar status da viagem!");
                }
            }
        }

        public List<Viagem> List(bool? todos)
        {
            List<Viagem> lista = new List<Viagem>();

            StringBuilder textoComando = new StringBuilder(
                "SELECT ID, CODIGO, MOTORISTAID, VEICULOID, INICIO, FIM, MOTIVOID, STATUSVIAGEMID, DATAINCLUSAO, DATAALTERACAO, STATUS FROM VIAGEM WHERE (1 = 1)"
                );

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                if (todos == false) { textoComando.Append(" AND STATUS IN ( :STATUS )"); Comando.Parameters.Add("STATUS", (int)ENUMSTATUS.ATIVO); }
                else if (todos == null) { textoComando.Append(" AND STATUS NOT IN ( :STATUS )"); Comando.Parameters.Add("STATUS", (int)ENUMSTATUS.ATIVO); }

                Conexao.Open();

                Comando.CommandText = textoComando.ToString();

                using (var DataReader = Comando.ExecuteReader())
                {
                    if (DataReader.HasRows)
                    {
                        while (DataReader.Read())
                        {
                            Viagem itemLista = new Viagem();
                            itemLista.ID = Convert.ToInt32(DataReader["ID"]);
                            itemLista.Codigo = Convert.ToString(DataReader["CODIGO"]);
                            itemLista.MotoristaViagem.GetByID(Convert.ToInt32(DataReader["MOTORISTAID"]), true);
                            itemLista.VeiculoViagem = VeiculoViagem.BuscarPorId(Convert.ToInt32(DataReader["VEICULOID"]), null);
                            itemLista.Inicio = Convert.ToDateTime(DataReader["INICIO"]);
                            if (DataReader["FIM"] != DBNull.Value)
                                itemLista.Fim = Convert.ToDateTime(DataReader["FIM"]);
                            itemLista.Motivo.Read(Convert.ToInt32(DataReader["MOTIVOID"]));
                            itemLista.ViagemStatus.Read(Convert.ToInt32(DataReader["STATUSVIAGEMID"]));
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
    }
}