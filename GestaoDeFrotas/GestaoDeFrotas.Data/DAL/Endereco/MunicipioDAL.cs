using GestaoDeFrotas.Data.DBENTITIES;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GestaoDeFrotas.Data.DAL
{
    public class MunicipioDAL : OracleBaseDAL
    {
        public IEnumerable<MunicipioDBE> ListarMunicipios()
        {
            List<MunicipioDBE> Municipios = new List<MunicipioDBE>();
            string comandoSql =
                "SELECT " +
                "M.ID, " +
                "M.NOME, " +
                "M.CODIGOIBGE, " +
                "M.DATAINCLUSAO, " +
                "M.DATAALTERACAO, " +
                "M.STATUS, " +
                "E.ID, " +
                "E.NOME, " +
                "E.UF, " +
                "E.CODIGOIBGE, " +
                "E.DATAINCLUSAO, " +
                "E.DATAALTERACAO, " +
                "E.STATUS " +
                "FROM " +
                "MUNICIPIO M " +
                "INNER JOIN " +
                "ESTADO E " +
                "ON M.ESTADOID = E.ID ";

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(comandoSql, Conexao))
            {
                Conexao.Open();

                using(var reader = Comando.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            MunicipioDBE municipio = new MunicipioDBE();
                            municipio.ID = reader.GetInt32(0);
                            municipio.NomeMunicipio = reader.GetString(1).ToUpper();
                            municipio.CodigoIbge = reader.GetString(2);
                            municipio.DataInclusao = reader.GetDateTime(3);
                            if (!reader.IsDBNull(4))
                                municipio.DataAlteracao = reader.GetDateTime(4);
                            municipio.Status = Convert.ToBoolean(reader.GetInt32(5));
                            municipio.Estado.ID = reader.GetInt32(6);
                            municipio.Estado.NomeEstado = reader.GetString(7).ToUpper();
                            municipio.Estado.UF = reader.GetString(8);
                            municipio.Estado.CodigoIbge = reader.GetString(9);
                            municipio.Estado.DataInclusao = reader.GetDateTime(10);
                            if (!reader.IsDBNull(11))
                                municipio.Estado.DataAlteracao = reader.GetDateTime(11);
                            municipio.Estado.Status = Convert.ToBoolean(reader.GetInt32(12));
                            Municipios.Add(municipio);
                        }
                    }
                }
            }
            
            List <MunicipioDBE> ListaOrdenada =  Municipios.OrderBy(o => o.NomeMunicipio).ToList();
            return ListaOrdenada;
        }
        public void Create(MunicipioDBE obj)
        {
            string comandoSql = "INSERT INTO MUNICIPIO (NOME, CODIGOIBGE, ESTADOID) VALUES ("
                + "\'" + obj.NomeMunicipio + "\'"
                + ", \'" + obj.CodigoIbge + "\', "
                + obj.Estado.ID + ")";

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(comandoSql, Conexao))
            {
                Conexao.Open();
                Comando.ExecuteNonQuery();
            }
        }

        public MunicipioDBE Read(int id)
        {
            MunicipioDBE retorno = new MunicipioDBE();

            StringBuilder comandoSQL = new StringBuilder("SELECT " +
                "M.ID, " +
                "M.NOME, " +
                "M.CODIGOIBGE, " +
                "E.ID, " +
                "E.NOME, " +
                "E.UF, " +
                "E.CODIGOIBGE " +
                "FROM " +
                "MUNICIPIO M " +
                "INNER JOIN " +
                "ESTADO E " +
                "ON M.ESTADOID = E.ID " +
                "WHERE M.ID = :ID");

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
                        retorno.ID = reader.GetInt32(0);
                        retorno.NomeMunicipio = reader.GetString(1).ToUpper();
                        retorno.CodigoIbge = reader.GetString(2);
                        retorno.Estado.ID = reader.GetInt32(3);
                        retorno.Estado.NomeEstado = reader.GetString(4).ToUpper();
                        retorno.Estado.UF = reader.GetString(5);
                        retorno.Estado.CodigoIbge = reader.GetString(6);
                    }
                }
            }

            return retorno;
        }
    }
}