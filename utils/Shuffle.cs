using System;
using System.Collections.Generic;
using System.Linq;

namespace Transacoes_blockchain.utils
{
  public static class Shuffle
  {
    private static readonly Random rng = new();

    public static IList<T> ShuffleList<T>(this IList<T> list)
    {
      int n = list.Count;
      while (n > 1)
      {
        n--;
        int k = rng.Next(n + 1);
        (list[n], list[k]) = (list[k], list[n]);
      }
      return list;
    }
  }
}
