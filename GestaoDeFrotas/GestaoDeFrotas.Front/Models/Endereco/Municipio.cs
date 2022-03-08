using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace CadastroDeCaminhoneiro.Models
{
    public class Municipio : OracleModel
    {
        public int ID { get; set; }
        [DisplayName("Município")]
        public string NomeMunicipio { get; set; }
        public Estado Estado { get; set; }
        [DisplayName("Codigo IBGE")]
        public string CodigoIbge { get; set; }
        [DisplayName("Data de inclusão")]
        public DateTime DataInclusao { get; set; }
        [DisplayName("Data de alteração")]
        public DateTime DataAlteracao { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }

        public Municipio()
        {
            this.Estado = new Estado();
        }
        public IEnumerable<Municipio> ListarMunicipios()
        {
            List<Municipio> Municipios = new List<Municipio>();
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
                            Municipio municipio = new Municipio();
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
            
            List <Municipio> ListaOrdenada =  Municipios.OrderBy(o => o.NomeMunicipio).ToList();
            return ListaOrdenada;
        }
        public void Create()
        {
            string comandoSql = "INSERT INTO MUNICIPIO (NOME, CODIGOIBGE, ESTADOID) VALUES ("
                + "\'" + this.NomeMunicipio + "\'"
                + ", \'" + this.CodigoIbge + "\', "
                + this.Estado.ID + ")";

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(comandoSql, Conexao))
            {
                Conexao.Open();
                Comando.ExecuteNonQuery();
            }
        }
    }
}