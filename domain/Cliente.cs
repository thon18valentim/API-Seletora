using Newtonsoft.Json;

namespace Transacoes_blockchain
{
  public class Cliente
  {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("nome")]
    public string? Nome { get; set; }

    [JsonProperty("senha")]
    public int Senha { get; set; }

    [JsonProperty("qtdMoeda")]
    public int QtdMoeda { get; set; }
  }
}
