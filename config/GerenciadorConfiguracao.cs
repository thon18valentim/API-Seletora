using Microsoft.Extensions.Configuration;

namespace Transacoes_blockchain.config
{
  public class GerenciadorConfiguracao
  {
    public string? UrlBase { get; set; }
    public string? HorarioUrl { get; set; }
  }

  public static class BuildGerenciadorConfiguracao
  {
    public static GerenciadorConfiguracao Build()
    {
      IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

      return config.GetRequiredSection("GerenciadorUrl").Get<GerenciadorConfiguracao>();
    }
  }
}
