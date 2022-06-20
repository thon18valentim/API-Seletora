using Newtonsoft.Json;

namespace Transacoes_blockchain.domain
{
  public class GerenciadorTimeStamp
  {
    [JsonProperty("objeto")]
    public DateTime Time { get; set; }
  }
}
