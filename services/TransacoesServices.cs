using Transacoes_blockchain.infra.api;
using Transacoes_blockchain.config;
using Transacoes_blockchain.infra.data;

namespace Transacoes_blockchain.services
{
  public class TransacoesServices
  {
    private readonly GerenciadorIntegracao gerenciadorIntegracao;
    private readonly GerenciadorConfiguracao gerenciadorConfiguracao;

    public TransacoesServices(GerenciadorIntegracao gerenciadorIntegracao)
    {
      this.gerenciadorIntegracao = gerenciadorIntegracao;
      gerenciadorConfiguracao = BuildGerenciadorConfiguracao.Build();
    }

    public void PrepararBanco()
    {
      DalHelper.CriarBancoSQLite();
      DalHelper.CriarTabelaTransacoesSQlite();
      DalHelper.CriarTabelaValidadoresSQlite();
    }
  }
}
