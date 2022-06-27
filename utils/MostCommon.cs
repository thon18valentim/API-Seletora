namespace Transacoes_blockchain.utils
{
  public static class MostCommon
  {
    public static T FindMostCommon<T>(this IEnumerable<T> list)
    {
      return list.GroupBy(i => i).OrderByDescending(grp => grp.Count())
      .Select(grp => grp.Key).First();
    }
  }
}
