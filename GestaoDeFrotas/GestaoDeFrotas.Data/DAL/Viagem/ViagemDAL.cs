using GestaoDeFrotas.Data.DBENTITIES;
using GestaoDeFrotas.Data.Enums;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GestaoDeFrotas.Data.DAL
{
    public class ViagemDAL : OracleBaseDAL, IDal<ViagemDBE>
    {
        public void Create(ViagemDBE obj)
        {
            StringBuilder textoComando = new StringBuilder("INSERT INTO VIAGEM (CODIGO, MOTORISTAID, VEICULOID, INICIO, FIM, MOTIVOID, STATUSVIAGEMID) " +
                "VALUES (:CODIGO, :MOTORISTAID, :VEICULOID, :INICIO, :FIM, :MOTIVOID, :STATUSVIAGEMID)");

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();
                Comando.CommandText = textoComando.ToString();
                Comando.Parameters.Add("CODIGO", obj.Codigo);
                Comando.Parameters.Add("MOTORISTAID", obj.MotoristaViagem.ID);
                Comando.Parameters.Add("VEICULOID", obj.VeiculoViagem.ID);
                Comando.Parameters.Add("INICIO", obj.Inicio);
                if (obj.Fim.Date == DateTime.MinValue.Date) Comando.Parameters.Add("FIM", null);
                else Comando.Parameters.Add("FIM", obj.Fim);
                Comando.Parameters.Add("MOTIVOID", obj.Motivo.ID);
                Comando.Parameters.Add("STATUSVIAGEMID", obj.ViagemStatus.ID);
                if (Comando.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Erro ao cadastrar viagem!");
                }
            }
        }

        public ViagemDBE Read(int id)
        {
            var retorno = new ViagemDBE();

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
                        retorno.ID = Convert.ToInt32(DataReader["ID"]);
                        retorno.Codigo = Convert.ToString(DataReader["CODIGO"]);
                        retorno.MotoristaViagem = new MotoristaDAL().Read(Convert.ToInt32(DataReader["MOTORISTAID"]));
                        retorno.VeiculoViagem = new VeiculoDAL().Read(Convert.ToInt32(DataReader["VEICULOID"]));
                        retorno.Inicio = Convert.ToDateTime(DataReader["INICIO"]);
                        if (DataReader["FIM"] != DBNull.Value)
                            retorno.Fim = Convert.ToDateTime(DataReader["FIM"]);
                        retorno.Motivo = new MotivoViagemDAL().Read(Convert.ToInt32(DataReader["MOTIVOID"]));
                        retorno.ViagemStatus = new StatusViagemDAL().Read(Convert.ToInt32(DataReader["STATUSVIAGEMID"]));
                        retorno.DataInclusao = Convert.ToDateTime(DataReader["DATAINCLUSAO"]);
                        if (DataReader["DATAALTERACAO"] != DBNull.Value)
                            retorno.DataAlteracao = Convert.ToDateTime(DataReader["DATAALTERACAO"]);
                        retorno.Status = Convert.ToBoolean(DataReader["STATUS"]);
                    }
                }
            }

            return retorno;
        }
        // TODO: testar
        public IEnumerable<ViagemDBE> Read(ViagemDBE obj)
        {
            var lista = new List<ViagemDBE>();

            StringBuilder textoComando = new StringBuilder(
                "SELECT ID, CODIGO, MOTORISTAID, VEICULOID, INICIO, FIM, MOTIVOID, STATUSVIAGEMID, DATAINCLUSAO, DATAALTERACAO, STATUS FROM VIAGEM WHERE (STATUS = 1)");

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                if (!string.IsNullOrEmpty(obj.Codigo)) { textoComando.Append(" AND CODIGO = :CODIGO"); Comando.Parameters.Add("CODIGO", obj.Codigo.ToUpper()); }
                if (obj.MotoristaViagem.ID > 0) { textoComando.Append(" AND MOTORISTAID = :MOTORISTAID"); Comando.Parameters.Add("MOTORISTAID", obj.MotoristaViagem.ID); }
                if (obj.VeiculoViagem.ID > 0) { textoComando.Append(" AND VEICULOID = :VEICULOID"); Comando.Parameters.Add("VEICULOID", obj.VeiculoViagem.ID); }
                if (obj.Inicio.Date != DateTime.MinValue.Date) { textoComando.Append(" AND INICIO = :INICIO"); Comando.Parameters.Add("INICIO", obj.Inicio); }
                if (obj.Fim.Date != DateTime.MinValue.Date) { textoComando.Append(" AND FIM = :FIM"); Comando.Parameters.Add("FIM", obj.Fim); }
                if (obj.Motivo.ID > 0) { textoComando.Append(" AND MOTIVOID = :MOTIVOID"); Comando.Parameters.Add("MOTIVOID", obj.Motivo.ID); }
                if (obj.ViagemStatus.ID > 0) { textoComando.Append(" AND STATUSVIAGEMID = :STATUSVIAGEMID"); Comando.Parameters.Add("STATUSVIAGEMID", obj.ViagemStatus.ID); }
                if (obj.DataInclusao.Date != DateTime.MinValue.Date) { textoComando.Append(" AND DATAINCLUSAO = :DATAINCLUSAO"); Comando.Parameters.Add("DATAINCLUSAO", obj.DataInclusao); }

                Comando.CommandText = textoComando.ToString();

                Conexao.Open();

                using (var reader = Comando.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            ViagemDBE itemLista = new ViagemDBE();
                            itemLista.ID = Convert.ToInt32(reader["ID"]);
                            itemLista.Codigo = Convert.ToString(reader["CODIGO"]);
                            itemLista.MotoristaViagem = new MotoristaDAL().Read(Convert.ToInt32(reader["MOTORISTAID"]));
                            itemLista.VeiculoViagem = new VeiculoDAL().Read(Convert.ToInt32(reader["VEICULOID"]));
                            itemLista.Inicio = Convert.ToDateTime(reader["INICIO"]);
                            if (reader["FIM"] != DBNull.Value)
                                itemLista.Fim = Convert.ToDateTime(reader["FIM"]);
                            itemLista.Motivo = new MotivoViagemDAL().Read(Convert.ToInt32(reader["MOTIVOID"]));
                            itemLista.ViagemStatus = new StatusViagemDAL().Read(Convert.ToInt32(reader["STATUSVIAGEMID"]));
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
        public void Update(ViagemDBE obj)
        {
            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();

                StringBuilder textoComando = new StringBuilder(
                "UPDATE VIAGEM SET ");

                if(string.IsNullOrEmpty(obj.Codigo) 
                    && obj.MotoristaViagem.ID <= 0 
                    && obj.VeiculoViagem.ID <= 0 
                    && obj.Inicio.Date == DateTime.MinValue.Date
                    && obj.Fim.Date == DateTime.MinValue.Date
                    && obj.Motivo.ID <= 0 
                    && obj.ViagemStatus.ID <= 0)
                {
                    throw new Exception("Nenhum parâmetro válido encontrado.");
                }

                if (obj.ID <= 0)
                    throw new Exception("ID inválido.");

                if (!string.IsNullOrEmpty(obj.Codigo)) { textoComando.Append("CODIGO = :CODIGO, "); Comando.Parameters.Add("CODIGO", obj.Codigo.ToUpper()); }
                if (obj.MotoristaViagem.ID > 0) { textoComando.Append("MOTORISTAID = :MOTORISTAID, "); Comando.Parameters.Add("MOTORISTAID", obj.MotoristaViagem.ID); }
                if (obj.VeiculoViagem.ID > 0) { textoComando.Append("VEICULOID = :VEICULOID, "); Comando.Parameters.Add("VEICULOID", obj.VeiculoViagem.ID); }
                if (obj.Inicio.Date != DateTime.MinValue.Date) { textoComando.Append("INICIO = :INICIO, "); Comando.Parameters.Add("INICIO", obj.Inicio); }
                if (obj.Fim.Date != DateTime.MinValue.Date) { textoComando.Append("FIM = :FIM, "); Comando.Parameters.Add("FIM", obj.Fim); }
                if (obj.Motivo.ID > 0) { textoComando.Append("MOTIVOID = :MOTIVOID, "); Comando.Parameters.Add("MOTIVOID", obj.Motivo.ID); }
                if (obj.ViagemStatus.ID > 0) { textoComando.Append("STATUSVIAGEMID = :STATUSVIAGEMID, "); Comando.Parameters.Add("STATUSVIAGEMID", obj.ViagemStatus.ID); }

                textoComando.Remove(textoComando.Length - 2, 2);
                textoComando.Append(" WHERE ID = :ID");
                Comando.Parameters.Add("ID", obj.ID);

                Comando.CommandText = textoComando.ToString();

                if (Comando.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Erro ao atualizar viagem!");
                }
            }
        }

        public void Delete(int id)
        {
            StringBuilder textoComando = new StringBuilder("DELETE FROM VIAGEM WHERE ID = :ID");

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(textoComando.ToString(), Conexao))
            {
                Conexao.Open();

                Comando.Parameters.Add("ID", id);

                if (Comando.ExecuteNonQuery() == 0)
                {
                    throw new Exception("Erro ao excluir viagem!");
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

        public List<ViagemDBE> List(bool? todos)
        {
            List<ViagemDBE> lista = new List<ViagemDBE>();

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
                            ViagemDBE itemLista = new ViagemDBE();
                            itemLista.ID = Convert.ToInt32(DataReader["ID"]);
                            itemLista.Codigo = Convert.ToString(DataReader["CODIGO"]);
                            itemLista.MotoristaViagem = new MotoristaDAL().Read(Convert.ToInt32(DataReader["MOTORISTAID"]));
                            itemLista.VeiculoViagem = new VeiculoDAL().Read(Convert.ToInt32(DataReader["VEICULOID"]));
                            itemLista.Inicio = Convert.ToDateTime(DataReader["INICIO"]);
                            if (DataReader["FIM"] != DBNull.Value)
                                itemLista.Fim = Convert.ToDateTime(DataReader["FIM"]);
                            itemLista.Motivo = new MotivoViagemDAL().Read(Convert.ToInt32(DataReader["MOTIVOID"]));
                            itemLista.ViagemStatus = new StatusViagemDAL().Read(Convert.ToInt32(DataReader["STATUSVIAGEMID"]));
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