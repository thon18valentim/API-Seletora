using Newtonsoft.Json;
using RestSharp;
using Transacoes_blockchain.config;
using Transacoes_blockchain.domain;

namespace Transacoes_blockchain.infra.api
{
  public class GerenciadorIntegracao
  {
    private RestClient restClient;
    private GerenciadorConfiguracao gerenciadorConfiguracao;

    public GerenciadorIntegracao()
    {
      gerenciadorConfiguracao = BuildGerenciadorConfiguracao.Build();
    }

    public async Task<string> ObterHorarioGerenciador()
    {
      try
      {
        restClient = new RestClient();
        var request = new RestRequest($"{gerenciadorConfiguracao.UrlBase}{gerenciadorConfiguracao.HorarioUrl}")
        {
          Method = Method.Get
        };

        var response = await restClient.GetAsync(request);
        var retorno = response.Content ?? "";

        return retorno;
      }
      catch
      {
        throw;
      }
    }
  }
}
