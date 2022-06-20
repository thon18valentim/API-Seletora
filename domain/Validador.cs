using Newtonsoft.Json;

namespace Transacoes_blockchain
{
  public class Validador
  {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("nome")]
    public string? Nome { get; set; }

    [JsonProperty("ip")]
    public string? Ip { get; set; }
  }
}
