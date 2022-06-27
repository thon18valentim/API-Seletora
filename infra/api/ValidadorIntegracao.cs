using RestSharp;
using Transacoes_blockchain.domain;
using Transacoes_blockchain.infra.data;

namespace Transacoes_blockchain.infra.api
{
  public class ValidadorIntegracao
  {
    private RestClient restClient;

    public async Task EnviarChaveUnica(string url, Chave chaveUnica)
    {
      try
      {
        var options = new RestClientOptions(url)
        {
          RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        };

        restClient = new RestClient(options);
        var request = new RestRequest()
        {
          Resource = url,
          Method = Method.Post
        };
        request.AddJsonBody(chaveUnica);

        await restClient.PostAsync(request);
      }
      catch
      {
        throw;
      }
    }

    public async Task<int> SelecionarValidador(string url, Transacao transacao)
    {
      try
      {
        var options = new RestClientOptions(url)
        {
          RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        };
        restClient = new RestClient(options);
        var request = new RestRequest(url)
        {
          Method = Method.Post
        };
        request.AddJsonBody(transacao);

        var response = await restClient.PostAsync(request);
        var retorno = int.Parse(response.Content ?? "2");

        return retorno;
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        return -1;
      }
    }
  }
}
