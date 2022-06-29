using GestaoDeFrotas.Data;
using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;
using GestaoDeFrotas.Data.Enums;
using GestaoDeFrotas.Shared.FiltroBusca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Business.BLL
{
    public class MotoristaBLL
    {
        private MotoristaDAL _motoristaDAL;
        private CNHDAL _CNHDAL;
        private EnderecoDAL _enderecoDAL;

        public MotoristaBLL()
        {
            _motoristaDAL = new MotoristaDAL();
            _CNHDAL = new CNHDAL();
            _enderecoDAL = new EnderecoDAL();
        }

        public void CadastrarMotorista (MotoristaDBE dadosMotorista, ref Resposta<MotoristaDBE> Resposta)
        {
            try
            {
                int idMotorista = 0;

                // Verifica se existe cadastro inativo com o mesmo CPF
                var motoristaInativoID = _motoristaDAL.GetByCPF(dadosMotorista.CPF, null).ID;
                if (motoristaInativoID > 0)
                {
                    Resposta.Mensagem = "O CPF inserido está vinculado a este cadastro inativo!";
                    Resposta.Status = EnumStatusResposta.Aviso;
                    Resposta.Retorno = new MotoristaDBE
                    {
                        ID = motoristaInativoID
                    };
                    return;
                }
                // Insere endereço
                _enderecoDAL.Create(dadosMotorista.Endereco);

                // Verifica se existe CNH
                switch (_CNHDAL.VerificaSeEstaCadastrado(ref idMotorista, dadosMotorista.CNH))
                {
                    // Não existe CNH com os mesmos dados
                    case false:
                        _CNHDAL.Create(dadosMotorista.CNH);

                        _motoristaDAL.Create(dadosMotorista);

                        Resposta.Mensagem = "Motorista cadastrado com sucesso!";
                        Resposta.Status = EnumStatusResposta.Sucesso;
                        break;
                    // Existe CNH associada a um motorista
                    case true:
                        Resposta.Mensagem = "Um ou mais dados da CNH inseridos estão associados a este motorista!";
                        Resposta.Status = EnumStatusResposta.Aviso;
                        Resposta.Retorno = new MotoristaDBE
                        {
                            ID = idMotorista
                        };
                        break;
                    // Existe CNH cadastrada mas não está vinculada a nenhum motorista (houve uma tentativa de cadastro do motorista mas ocorreu um erro)
                    case null:
                        _motoristaDAL.Create(dadosMotorista);

                        Resposta.Mensagem = "Motorista cadastrado com sucesso!";
                        Resposta.Status = EnumStatusResposta.Sucesso;
                        break;
                }
            }
            // Verificar inner exception
            catch (CadastroEnderecoException e)
            {
                Resposta.Mensagem = e.Message;
                Resposta.Status = EnumStatusResposta.Erro;
            }
            catch (CadastroCNHException e)
            {
                Resposta.Mensagem = e.Message;
                Resposta.Status = EnumStatusResposta.Erro;

                if (!this.ExcluirEndereco(dadosMotorista.Endereco.ID))
                {
                    Resposta.Mensagem = e.Message + " / Erro ao excluir endereço com ID: " + dadosMotorista.Endereco.ID;
                }
            }
            catch (CadastroMotoristaException e)
            {
                Resposta.Mensagem = e.Message;
                Resposta.Status = EnumStatusResposta.Erro;

                if (!this.ExcluirEndereco(dadosMotorista.Endereco.ID))
                {
                    Resposta.Mensagem = e.Message + " / Erro ao excluir endereço com ID: " + dadosMotorista.Endereco.ID;
                }
            }
        }

        public IEnumerable<MotoristaDBE> BuscarDadosPainel(FiltroBusca Filtro, ENUMCAMPOSPAINELMOTORISTAS CampoOrdenacao, ENUMOPCOESORDENACAO Ordem)
        {
            var retorno = _motoristaDAL.Read(Filtro);

            if (Ordem == ENUMOPCOESORDENACAO.CRESCENTE)
            {
                switch (CampoOrdenacao)
                {
                    case ENUMCAMPOSPAINELMOTORISTAS.NOME:
                        retorno = retorno.OrderBy(m => m.PrimeiroNome);
                        break;
                    case ENUMCAMPOSPAINELMOTORISTAS.CPF:
                        retorno = retorno.OrderBy(m => m.CPF);
                        break;
                    case ENUMCAMPOSPAINELMOTORISTAS.CNH:
                        retorno = retorno.OrderBy(m => m.CNH);
                        break;
                    case ENUMCAMPOSPAINELMOTORISTAS.MUNICIPIO:
                        retorno = retorno.OrderBy(m => m.Endereco.Municipio.NomeMunicipio);
                        break;
                    case ENUMCAMPOSPAINELMOTORISTAS.DATAINCLUSAO:
                        retorno = retorno.OrderBy(m => m.DataInclusao);
                        break;
                    case ENUMCAMPOSPAINELMOTORISTAS.DATAALTERACAO:
                        retorno = retorno.OrderBy(m => m.DataAlteracao);
                        break;
                    case ENUMCAMPOSPAINELMOTORISTAS.STATUS:
                        retorno = retorno.OrderBy(m => m.Status);
                        break;
                    default:
                        retorno = retorno.OrderBy(m => m.PrimeiroNome);
                        break;
                }
            }
            else
            {
                switch (CampoOrdenacao)
                {
                    case ENUMCAMPOSPAINELMOTORISTAS.NOME:
                        retorno = retorno.OrderByDescending(m => m.PrimeiroNome);
                        break;
                    case ENUMCAMPOSPAINELMOTORISTAS.CPF:
                        retorno = retorno.OrderByDescending(m => m.CPF);
                        break;
                    case ENUMCAMPOSPAINELMOTORISTAS.CNH:
                        retorno = retorno.OrderByDescending(m => m.CNH);
                        break;
                    case ENUMCAMPOSPAINELMOTORISTAS.MUNICIPIO:
                        retorno = retorno.OrderByDescending(m => m.Endereco.Municipio.NomeMunicipio);
                        break;
                    case ENUMCAMPOSPAINELMOTORISTAS.DATAINCLUSAO:
                        retorno = retorno.OrderByDescending(m => m.DataInclusao);
                        break;
                    case ENUMCAMPOSPAINELMOTORISTAS.DATAALTERACAO:
                        retorno = retorno.OrderByDescending(m => m.DataAlteracao);
                        break;
                    case ENUMCAMPOSPAINELMOTORISTAS.STATUS:
                        retorno = retorno.OrderByDescending(m => m.Status);
                        break;
                    default:
                        retorno = retorno.OrderByDescending(m => m.PrimeiroNome);
                        break;
                }
            }

            return retorno;
        }

        private bool ExcluirEndereco(int id)
        {
            try
            {
                _enderecoDAL.Delete(id);
                return true;
            }
            catch (CadastroEnderecoException)
            {
                return false;
            }
        }
    }
}
