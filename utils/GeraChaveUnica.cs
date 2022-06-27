using Transacoes_blockchain.infra.data;

namespace Transacoes_blockchain.utils
{
  public class GeraChaveUnica
  {
    static string[] letras = { "a", "b", "c", "d", "e", "f", "g", "h" };

    public static string GerarChaveUnica()
    {
      var chavesCriadas = DalHelper.GetChaveUnica();

      bool run = true;
      var code = "";
      int count;
      while (run)
      {
        count = 0;
        while(count < 5)
        {
          Random rd = new();
          code += letras[rd.Next(0, 7)];
          count++;
        }

        if (!chavesCriadas.Contains("code"))
        {
          return code;
        }
        code = "";
      }

      return code;
    }
  }
}
