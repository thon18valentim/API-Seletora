using Newtonsoft.Json;

namespace Transacoes_blockchain.domain
{
  public class GerenciadorTimeStamp
  {
    [JsonProperty("time")]
    public DateTime Time { get; set; }
  }
}
