namespace Transacoes_blockchain.utils
{
  public static class SyncTime
  {
    public static int ToUnixTimestamp(this DateTime value)
    {
      return (int)Math.Truncate((value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
    }

    public static int UnixTimestamp(this DateTime value)
    {
      return (int)Math.Truncate((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
    }

    /**
     * @tempoRecebido -> Horário enviado pelo gerenciador
     * @horarioRequisicao -> Horário da realização da requisição
     * @horarioRecebimento -> Horário do recebimento da requisição
     * @return -> Horário sincronizado
     */
    public static float CristianSyncTime(float tempoRecebido, float horarioRequisicao, float horarioRecebimento)
    {
      return tempoRecebido + (horarioRecebimento - horarioRequisicao) / 2;
    }
  }
}
