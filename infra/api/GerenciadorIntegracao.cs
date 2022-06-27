using RestSharp;
using Transacoes_blockchain.config;
using Microsoft.AspNetCore.Mvc;

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

    public async Task<string> AtualizarStatusTransacao(Transacao transacao)
    {
      try
      {
        restClient = new RestClient();
        var request = new RestRequest($"{gerenciadorConfiguracao.UrlBase}{gerenciadorConfiguracao.Transacao}/{transacao.Id}/{transacao.Status}")
        {
          Method = Method.Post
        };

        var response = await restClient.PostAsync(request);
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
