using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace GestaoDeFrotas
{
    public class OracleModel
    {
        protected readonly string stringConexao = ConfigurationManager.AppSettings["StringOracle"];
        protected OracleConnection Conexao { get; set; }
        protected OracleCommand Comando { get; set; }
        protected OracleDataReader DataReader { get; set; }
    }
}