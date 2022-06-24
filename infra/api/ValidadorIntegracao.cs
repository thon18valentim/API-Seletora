using RestSharp;
using Transacoes_blockchain.infra.data;

namespace Transacoes_blockchain.infra.api
{
  public class ValidadorIntegracao
  {
    private RestClient restClient;

    public async Task<string> SelecionarValidador(string url)
    {
      try
      {
        restClient = new RestClient();
        var request = new RestRequest()
        {
          Resource = url,
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
