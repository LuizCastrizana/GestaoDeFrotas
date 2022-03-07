using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using Oracle.ManagedDataAccess.Client;

namespace CadastroDeCaminhoneiro.Models
{
    public class Endereco : OracleModel, IModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(100)]
        [DisplayName("Logradouro")]
        public string Logradouro { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#}")]
        [Range(0, int.MaxValue, ErrorMessage = "Valor Inválido")]
        [DisplayName("Número")]
        public int? Numero { get; set; }

        [StringLength(20)]
        [DisplayName("Complemento")]
        public string Complemento { get; set; }

        [StringLength(50)]
        [DisplayName("Bairro")]
        public string Bairro { get; set; }

        public Municipio Municipio { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression(@"[0-9]{5}-[0-9]{3}", ErrorMessage = "Valor inválido")]
        [DisplayName("CEP")]
        public string Cep { get; set; }

        [DisplayName("Data de inclusão")]
        public DateTime DataInclusao { get; set; }

        [DisplayName("Data de alteração")]
        public DateTime DataAlteracao { get; set; }

        [DisplayName("Status")]
        public bool Status { get; set; }

        public Endereco()
        {
            this.Municipio = new Municipio();
            this.Complemento = "";
            this.Bairro = "";
        }
        /// <summary>
        /// Insere endereço no banco de dados com os valores do objeto
        /// </summary>
        /// <returns>ID do endereço inserido</returns>
        public void Create()
        {
            string comandoSql = "INSERT INTO ENDERECO E (E.LOGRADOURO, E.NUMERO, E.COMPLEMENTO, E.BAIRRO, E.MUNICIPIOID, E.CEP) " +
                                "VALUES (:LOGRADOURO, :NUMERO, :COMPLEMENTO, :BAIRRO, :MUNICIPIOID, :CEP) RETURNING ID INTO :ID";
            int idNovo;
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(null, Conexao))
                {
                    Conexao.Open();

                    Comando.Parameters.Add("LOGRADOURO", Logradouro.ToUpper());
                    if (Numero.HasValue) Comando.Parameters.Add("NUMERO", Numero);
                    else Comando.Parameters.Add("NUMERO", null);
                    if (Complemento != null) Comando.Parameters.Add("COMPLEMENTO", Complemento.ToUpper());
                    else Comando.Parameters.Add("COMPLEMENTO", null);
                    if (Bairro != null) Comando.Parameters.Add("BAIRRO", Bairro.ToUpper());
                    else Comando.Parameters.Add("BAIRRO", null);
                    Comando.Parameters.Add("MUNICIPIOID", Municipio.ID);
                    Comando.Parameters.Add("CEP", Cep);

                    Comando.Parameters.Add(new OracleParameter
                    {
                        ParameterName = "ID",
                        OracleDbType = OracleDbType.Decimal,
                        Direction = ParameterDirection.Output
                    });

                    Comando.CommandText = comandoSql;
                    if (Comando.ExecuteNonQuery() == 1)
                        idNovo = Convert.ToInt32(Comando.Parameters["ID"].Value.ToString());
                    else
                        throw new CadastroEnderecoException("Erro ao icluir endereço!");
                } 
                ID = idNovo;
            }
            catch (Exception e)
            {
                throw new CadastroEnderecoException("Erro ao incluir endereço", e.InnerException);
            }
        }
        /// <summary>
        /// Exclui endereço
        /// </summary>
        /// <param name="id">ID do endereço a ser excluido</param>
        public void Delete(int id)
        {
            string comandoSql = "DELETE FROM ENDERECO WHERE ID = " + id.ToString(); ;
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(comandoSql, Conexao))
                {
                    Conexao.Open();
                    if (Comando.ExecuteNonQuery() < 1)
                        throw new CadastroEnderecoException("Erro ao excluir endereço!");
                }
            }
            catch (Exception e)
            {
                throw new CadastroEnderecoException("Erro ao excluir endereço", e.InnerException, Convert.ToUInt32(id));
            }
        }
        /// <summary>
        /// Atualiza o endereço com os parametros do objeto 
        /// </summary>
        public void Update()
        {
            string comandoSql = "UPDATE ENDERECO E " +
                    "SET E.LOGRADOURO = " + "\'" + this.Logradouro.ToUpper() + "\'";
            comandoSql += this.Numero is null ? ", E.NUMERO = NULL " : ", E.NUMERO = " + this.Numero;
            comandoSql += this.Complemento is null ? ", E.COMPLEMENTO = \'\' " : ", E.COMPLEMENTO = \'" + this.Complemento.ToUpper() + "\'";
            comandoSql += this.Bairro is null ? ", E.BAIRRO = \'\' " : ", E.BAIRRO = \'" + this.Bairro.ToUpper() + "\'";
            comandoSql +=
                ", E.CEP =  " + "\'" + this.Cep.ToUpper() + "\'" +
                ", E.MUNICIPIOID = " + this.Municipio.ID +
                " WHERE E.ID = " + this.ID;
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(comandoSql, Conexao))
                {
                    Conexao.Open();
                    if (Comando.ExecuteNonQuery() < 1)
                        throw new CadastroEnderecoException("Erro ao editar endereço!");
                }
            }
            catch (Exception e)
            {
                throw new CadastroEnderecoException("Erro ao editar endereço!", e);
            }
        }

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