using GestaoDeFrotas.Data;
using GestaoDeFrotas.Data.DAL;
using GestaoDeFrotas.Data.DBENTITIES;
using GestaoDeFrotas.Data.Enums;
using GestaoDeFrotas.Shared.Enums;
using GestaoDeFrotas.Shared.FiltroBusca;
using GestaoDeFrotas.Shared.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoDeFrotas.Business.BLL
{
    public class MotoristaBLL
    {
        private readonly MotoristaDAL _motoristaDAL;
        private readonly CNHDAL _CNHDAL;
        private readonly EnderecoDAL _enderecoDAL;
        private readonly VeiculoDAL _veiculoDAL;

        public MotoristaBLL()
        {
            _motoristaDAL = new MotoristaDAL();
            _CNHDAL = new CNHDAL();
            _enderecoDAL = new EnderecoDAL();
            _veiculoDAL = new VeiculoDAL();
        }

        #region Busca
        public MotoristaDBE BuscarPorID(int id)
        {
            var retorno = _motoristaDAL.Read(id);
            //retorno.ListaVeiculos = _veiculoDAL.ListarVeiculosPorIDMotorista(id, true);
            return retorno;
        }

        public IEnumerable<VeiculoDBE> ListarVeiculosDoMotorista(int MotoristaID)
        {
            return _veiculoDAL.ListarVeiculosPorIDMotorista(MotoristaID, true);
        }

        public IEnumerable<MotoristaDBE> BuscarDadosPainel(FiltroBusca Filtro, ENUMCAMPOSPAINELMOTORISTAS CampoOrdenacao, ENUMOPCOESORDENACAO Ordem)
        {
            var retorno = _motoristaDAL.List(Filtro);

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

        public IEnumerable<MotoristaDBE> BuscarPorNomeOuCPF(string busca, bool? todos)
        {
            if (busca == null)
                busca = string.Empty;
            var lista = new MotoristaDAL().List(todos).
                Where(m => (m.PrimeiroNome + " " + m.Sobrenome).ToUpper().Contains(busca.ToUpper())
                ||
                (StringTools.RemoverCaracteres(m.CPF, "-.")).Contains(StringTools.RemoverCaracteres(busca, "-.")));

            return lista;
        }
        #endregion

        #region Cadastro
        public void CadastrarMotorista(MotoristaDBE DadosMotorista, ref RespostaNegocio<MotoristaDBE> Resposta)
        {
            try
            {
                int idMotorista = 0;

                // Verifica se existe cadastro inativo com o mesmo CPF
                var motoristaInativoID = _motoristaDAL.GetByCPF(DadosMotorista.CPF, null).ID;
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
                _enderecoDAL.Create(DadosMotorista.Endereco);

                // Verifica se existe CNH
                switch (_CNHDAL.VerificaSeEstaCadastrado(ref idMotorista, DadosMotorista.CNH))
                {
                    // Não existe CNH com os mesmos dados
                    case false:
                        _CNHDAL.Create(DadosMotorista.CNH);

                        _motoristaDAL.Create(DadosMotorista);

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
                        _motoristaDAL.Create(DadosMotorista);

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

                if (!this.ExcluirEndereco(DadosMotorista.Endereco.ID))
                {
                    Resposta.Mensagem = e.Message + " / Erro ao excluir endereço com ID: " + DadosMotorista.Endereco.ID;
                }
            }
            catch (CadastroMotoristaException e)
            {
                Resposta.Mensagem = e.Message;
                Resposta.Status = EnumStatusResposta.Erro;

                if (!this.ExcluirEndereco(DadosMotorista.Endereco.ID))
                {
                    Resposta.Mensagem = e.Message + " / Erro ao excluir endereço com ID: " + DadosMotorista.Endereco.ID;
                }
            }
        }

        public void EditarMotorista(MotoristaDBE DadosMotorista, ref RespostaNegocio<MotoristaDBE> Resposta)
        {
            try
            {
                new EnderecoDAL().Update(DadosMotorista.Endereco);
                new CNHDAL().Update(DadosMotorista.CNH);
                new MotoristaDAL().Update(DadosMotorista);

                Resposta.Mensagem = "Cadastro editado com sucesso!";
                Resposta.Status = EnumStatusResposta.Sucesso;
            }
            // Verificar inner exception
            catch (Exception e)
            {
                Resposta.Mensagem = e.Message + " " + e.InnerException.Message;
                Resposta.Status = EnumStatusResposta.Erro;
            }
        }

        public void AlterarStatusMotorista(int id, bool status, ref RespostaNegocio<MotoristaDBE> Resposta)
        {
            try
            {
                new MotoristaDAL().AtualizarStatus(id, !status);
                Resposta.Mensagem = "Cadastro atualizado com sucesso!";
                Resposta.Status = EnumStatusResposta.Sucesso;
            }
            catch (Exception e)
            {
                Resposta.Mensagem = e.Message;
                Resposta.Status = EnumStatusResposta.Erro;
            }
        }

        public void VincularVeiculo(int MotoristaID, string Placa, ref RespostaNegocio<MotoristaDBE> Resposta)
        {
            MotoristaDBE Motorista = new MotoristaDBE();
            VeiculoDBE Veiculo = new VeiculoDBE();
            bool valido = true;
            try
            {
                Motorista = new MotoristaDAL().Read(MotoristaID);
                Veiculo = new VeiculoDAL().BuscarPorPlaca(Placa.ToUpper(), true);
            }
            catch (Exception e)
            {
                Resposta.Mensagem = "Erro ao buscar dados de motorista e veículo - " + e.Message;
                Resposta.Status = EnumStatusResposta.Erro;
                valido = false;
            }
            if (Veiculo.ID == 0)
            {
                Resposta.Mensagem = "Veículo não encontrado ou inativo! Placa: " + Placa;
                Resposta.Status = EnumStatusResposta.ErroValidacao;
                valido = false;
            }
            if (Motorista.ListaVeiculos.Any(x => x.Placa == Placa.ToUpper()))
            {
                Resposta.Mensagem = "O Veículo informado já está vinculado ao motorista!";
                Resposta.Status = EnumStatusResposta.ErroValidacao;
                valido = false;
            }
            if (valido)
            {
                try
                {
                    new MotoristaDAL().VincularVeiculoMotorista(Veiculo.ID, Motorista.ID);
                    Resposta.Mensagem = "Veículo vinculado com sucesso!";
                    Resposta.Status = EnumStatusResposta.Sucesso;
                }
                catch (Exception e)
                {
                    Resposta.Mensagem = "Erro ao vincular veículo - " + e.Message;
                    Resposta.Status = EnumStatusResposta.Erro;
                } 
            }
        }

        public void DesvincularVeiculoMotorista(int VeiculoID, int MotoristaID, ref RespostaNegocio<MotoristaDBE> Resposta)
        {
            try
            {
                new MotoristaDAL().DesvincularVeiculoMotorista(VeiculoID, MotoristaID);
                Resposta.Mensagem = "Veículo desvinculado com sucesso!";
                Resposta.Status = EnumStatusResposta.Sucesso;
            }
            catch (Exception e)
            {
                Resposta.Mensagem = "Erro ao desvincular veículo - " + e.Message;
                Resposta.Status = EnumStatusResposta.Erro;
            }
        }

        public List<MunicipioDBE> BuscarMunicipiosPorEstado(int EstadoID)
        {
            return new MunicipioDAL().ListarMunicipios()
                .Where(x => x.Estado.ID == EstadoID)
                .OrderBy(x => x.NomeMunicipio)
                .ToList();
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
        
        #endregion

        #region Dados Menus DropDown
        public DropDownMenuPainelMotorista MontarMenusPainel()
        {
            var retorno = new DropDownMenuPainelMotorista
            {
                Ordenacao = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMOPCOESORDENACAO.CRESCENTE, "Crescente"),
                    new DropDownItem((int)ENUMOPCOESORDENACAO.DECRESCENTE, "Decrescente")
                },
                Filtros = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.NOME, "Nome"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.CPF, "CPF"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.CNH, "CNH"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.MUNICIPIO, "Município"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.DATAINCLUSAO, "Data inclusão"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.DATAALTERACAO, "Data alteração"),
                },
                CampoOrdenacao = new List<DropDownItem>
                {
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.NOME, "Nome"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.CPF, "CPF"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.CNH, "CNH"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.MUNICIPIO, "Município"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.DATAINCLUSAO, "Data inclusão"),
                    new DropDownItem((int)ENUMCAMPOSPAINELMOTORISTAS.DATAALTERACAO, "Data alteração"),
                },
            };

            return retorno;
        }

        public DropDownMenuCadastroMotorista MontarMenusCadastro(bool flgMunicipios)
        {
            var retorno = new DropDownMenuCadastroMotorista
            {
                Municipios = Enumerable.Empty<MunicipioDBE>(),
                Estados = new EstadoDAL().List(),
                CategoriasCNH = new CategoriaCNHDAL().List()
            };

            if (flgMunicipios)
                retorno.Municipios = new MunicipioDAL().ListarMunicipios();

            return retorno;
        }
    }

    public class DropDownMenuPainelMotorista
    {
        public List<DropDownItem> Ordenacao { get; set; }
        public List<DropDownItem> Filtros { get; set; }
        public List<DropDownItem> CampoOrdenacao { get; set; }
    }

    public class DropDownMenuCadastroMotorista
    {
        public IEnumerable<CategoriaCNHDBE> CategoriasCNH { get; set; }
        public IEnumerable<EstadoDBE> Estados { get; set; }
        public IEnumerable<MunicipioDBE> Municipios { get; set; }
    }
    #endregion
}
