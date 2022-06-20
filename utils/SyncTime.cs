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
  }
}
