using Newtonsoft.Json;

namespace Transacoes_blockchain.domain
{
  public class Chave
  {
    [JsonProperty("chaveUnica")]
    public string ChaveUnica { get; set; }

    public Chave(string chaveUnica)
    {
      ChaveUnica = chaveUnica;
    }
  }
}
