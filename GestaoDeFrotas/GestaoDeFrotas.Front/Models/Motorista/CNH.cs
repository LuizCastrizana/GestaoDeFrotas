using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace CadastroDeCaminhoneiro.Models
{
    public class CNH : OracleModel, IModel
    {
        [DisplayName("ID")]
        public int ID { get; set; }

        [StringLength(11)]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "Inserir 11 dígitos, apenas números")]
        [DisplayName("Número registro")]
        public string Numero { get; set; }

        [StringLength(11)]
        [RegularExpression(@"^[A-Za-z]{2}[0-9]{9}$", ErrorMessage = "RENACH Inválido")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("RENACH")]
        public string RENACH { get; set; }

        [StringLength(10)]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Inserir 10 dígitos, apenas números")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Espelho")]
        public string Espelho { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Data de emissão")]
        public DateTime DataEmissao { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [DisplayName("Data de vencimento")]
        public DateTime DataValidade { get; set; }

        public CategoriaCNH Categoria { get; set; }

        [DisplayName("Data de inclusão")]
        public DateTime DataInclusao { get; set; }

        [DisplayName("Data de alteração")]
        public DateTime DataAlteracao { get; set; }

        [DisplayName("Status")]
        public bool Status { get; set; }

        public CNH()
        {
            this.Categoria = new CategoriaCNH();
        }

        // TODO: Testar todos os métodos
        // TODO: Usar parameters

        public void Read(int id)
        {
            string comandoSql = "SELECT "
                + "C.ID, "
                + "C.NUMERO, "
                + "C.RENACH, "
                + "C.ESPELHO, "
                + "C.DATAEMISSAO, "
                + "C.DATAVALIDADE, "
                + "C.DATAINCLUSAO, "
                + "C.DATAALTERACAO, "
                + "C.STATUS, "
                + "C.CATEGORIAID, "
                + "CA.CATEGORIA "
                + "FROM CNH C "
                + "INNER JOIN CATEGORIACNH CA "
                + "ON C.CATEGORIAID = CA.ID "
                + "WHERE C.ID = :ID";
            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(comandoSql, Conexao))
            {
                Conexao.Open();
                Comando.Parameters.Add("ID", id);
                using (var oracle = Comando.ExecuteReader())
                {
                    if (oracle.HasRows)
                    {
                        oracle.Read();
                        this.ID = oracle.GetInt32(0);
                        this.Numero = oracle.GetString(1);
                        this.RENACH = oracle.GetString(2);
                        this.Espelho = oracle.GetString(3);
                        this.DataEmissao = oracle.GetDateTime(4);
                        this.DataValidade = oracle.GetDateTime(5);
                        this.DataInclusao = oracle.GetDateTime(6);
                        if (!oracle.IsDBNull(7))
                            this.DataAlteracao = oracle.GetDateTime(7);
                        this.Status = Convert.ToBoolean(oracle.GetInt32(8));
                        this.Categoria.ID = oracle.GetInt32(9);
                        this.Categoria.Categoria = oracle.GetString(10);
                    }
                }
            }
        }
        public void Create()
        {
            var comandoSql = "INSERT INTO CNH C (C.NUMERO, C.RENACH, C.ESPELHO, C.DATAEMISSAO, C.DATAVALIDADE, C.CATEGORIAID) " +
                        "VALUES (:NUMERO, :RENACH, :ESPELHO, :DATAEMISSAO, :DATAVALIDADE, :CATEGORIAID) RETURNING ID INTO :ID";
            int idNovo;
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(comandoSql, Conexao))
                {
                    Conexao.Open();

                    Comando.Parameters.Add("NUMERO", Numero.ToUpper());
                    Comando.Parameters.Add("RENACH", RENACH.ToUpper());
                    Comando.Parameters.Add("ESPELHO", Espelho.ToUpper());
                    Comando.Parameters.Add("DATAEMISSAO", DataEmissao);
                    Comando.Parameters.Add("DATAVALIDADE", DataAlteracao);
                    Comando.Parameters.Add("CATEGORIAID", Categoria.ID);

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
                        throw new CadastroCNHException("Erro ao icluir CNH!");
                }
            ID = idNovo;
            }
            catch (Exception e)
            {
                throw new CadastroCNHException("Erro ao incluir CNH", e);
            }
        }
        public void Update()
        {
            string comandoSql = "UPDATE CNH C SET "
                + "C.NUMERO = " + "\'" + this.Numero + "\', "
                + "C.RENACH = " + "\'" + this.RENACH + "\', "
                + "C.ESPELHO = " + "\'" + this.Espelho + "\', "
                + "C.DATAEMISSAO = " + "\'" + this.DataEmissao.ToString("dd/MM/yyyy") + "\', "
                + "C.DATAVALIDADE = " + "\'" + this.DataValidade.ToString("dd/MM/yyyy") + "\', "
                + "C.CATEGORIAID = "  + this.Categoria.ID + " "
                + "WHERE C.ID = " + this.ID;
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(comandoSql, Conexao))
                {
                    Conexao.Open();
                    if (Comando.ExecuteNonQuery() == 0)
                        throw new CadastroCNHException("Erro ao atualizar CNH!");
                }
            }
            catch (Exception e)
            {
                throw new CadastroCNHException("Erro ao atualizar CNH!", e);
            }
        }
        public void Delete(int id)
        {
            string comandoSql = "DELETE FROM CNH C WHERE C.ID = :ID";
            try
            {
                using (Conexao = new OracleConnection(stringConexao))
                using (Comando = new OracleCommand(comandoSql, Conexao))
                {
                    Conexao.Open();
                    Comando.Parameters.Add("ID", id);
                    if (Comando.ExecuteNonQuery() == 0)
                        throw new CadastroCNHException("Erro ao atualizar CNH!");
                }
            }
            catch (Exception e)
            {
                throw new CadastroCNHException("Erro ao excluir CNH!", e);
            }
        }
        /// <summary>
        /// Verifica se os dados da CNH já existem no banco de dados. Se existe insere o valor do ID na propriedade do objeto/>
        /// </summary>
        /// <param name="idMotorista">Referencia para uma variavel que armazena o ID do motorista ao qual está associada a CNH caso exista</param>
        /// <returns>true: existe CNH vinculada a um motorista | false: não existe CNH cadastrada | null: existe CNH mas não está associada a nenhum motorista</returns>
        public bool? VerificaSeEstaCadastrado(ref int idMotorista)
        {
            string buscaCNH = "SELECT ID, NUMERO, RENACH, ESPELHO, DATAEMISSAO, DATAVALIDADE FROM CNH WHERE "
                                + "NUMERO = \'" + this.Numero + "\' OR RENACH = \'" + this.RENACH + "\' OR ESPELHO = \'" + this.Espelho + "\'";
            string buscaMotorista = "SELECT ID FROM MOTORISTA WHERE CNHID = ";

            using (Conexao = new OracleConnection(stringConexao))
            using (Comando = new OracleCommand(null, Conexao))
            {
                Conexao.Open();
                Comando.CommandText = buscaCNH;
                using (var reader = Comando.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        this.ID = reader.GetInt32(0);
                        buscaMotorista += this.ID;
                    }
                    else
                    {
                        // não existe CNH cadastrada com os dados passados no objeto
                        return false;
                    }
                }
                // Existe CNH com algum dos dados passados no objeto
                Comando.CommandText = buscaMotorista;
                using (var reader = Comando.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        // existe CNH e está associada a um motorista
                        idMotorista = reader.GetInt32(0);
                        return true;
                    }
                    else
                    {
                        // Existe CNH cadastrada com algum dos dados do objeto, mas não está associada a nenhum motorista
                        return null;
                    }
                }
            }
        }

        public void UpdateStatus(int id, bool status)
        {
            throw new NotImplementedException();
        }
    }
}