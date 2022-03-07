using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Xunit;

namespace CadastroDeCaminhoneiro.Models
{
    public class CategoriaCNH : OracleModel
    {
        [DisplayName("ID")]
        public int ID{ get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Categoria")]
        public string Categoria { get; set; }
        [DisplayName("Data de inclusão")]
        public DateTime DataInclusao { get; set; }
        [DisplayName("Data de alteração")]
        public DateTime DataAlteracao { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }

        public CategoriaCNH()
        {

        }

        public IEnumerable<CategoriaCNH> List()
        {
            List<CategoriaCNH> lista = new List<CategoriaCNH>();
            IEnumerable<CategoriaCNH> listaOrdenada = new List<CategoriaCNH>();
            string comandoSql = "SELECT ID, CATEGORIA, DATAINCLUSAO, DATAALTERACAO, STATUS FROM CATEGORIACNH";
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
                            CategoriaCNH categoria = new CategoriaCNH();
                            categoria.ID = reader.GetInt32(0);
                            categoria.Categoria = reader.GetString(1);
                            categoria.DataInclusao = reader.GetDateTime(2);
                            if (!reader.IsDBNull(3))
                                categoria.DataAlteracao = reader.GetDateTime(3);
                            categoria.Status = Convert.ToBoolean(reader.GetInt32(4));
                            lista.Add(categoria);
                        }
                    } 
                }
            }
            listaOrdenada = lista.OrderBy(m => m.ID);
            return lista;
        }
    }
}