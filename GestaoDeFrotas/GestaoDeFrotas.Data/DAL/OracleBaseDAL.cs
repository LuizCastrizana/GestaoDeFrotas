using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Data.DAL
{
    public class OracleBaseDAL
    {
        protected readonly string stringConexao = ConfigurationManager.AppSettings["StringOracle"];
        protected OracleConnection Conexao { get; set; }
        protected OracleCommand Comando { get; set; }
        protected OracleDataReader DataReader { get; set; }
    }
}
