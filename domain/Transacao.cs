using Newtonsoft.Json;

namespace Transacoes_blockchain
{
  public class Transacao
  {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("remetente")]
    public int Remetente { get; set; }

    [JsonProperty("recebedor")]
    public int Recebedor { get; set; }

    [JsonProperty("valor")]
    public int Valor { get; set; }

    [JsonProperty("status")]
    public int Status { get; set; }

    /**
     * Valida os campos da transacao 
    */
    public bool Validate()
    {
      if(Status < 0 || Status > 3)
      {
        return false;
      }
      return true;
    }
  }
}
