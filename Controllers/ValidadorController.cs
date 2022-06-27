using Microsoft.AspNetCore.Mvc;
using Transacoes_blockchain.infra.api;
using Transacoes_blockchain.infra.data;
using Transacoes_blockchain.utils;
using Transacoes_blockchain.domain;

namespace Transacoes_blockchain.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class ValidadorController: ControllerBase
  {
    [HttpPost]
    public async Task<ActionResult> InserirValidador(Validador validador)
    {
      try
      {
        ValidadorIntegracao validadorIntegracao = new();

        DalHelper.AddValidador(validador);

        var chave = GeraChaveUnica.GerarChaveUnica();
        DalHelper.AddChaveUnica(validador.Id, chave);
        Chave chaveUnica = new(chave);
        var url = validador.Ip.Replace("-", ".");
        await validadorIntegracao.EnviarChaveUnica($"https://{url}/Chave", chaveUnica);

        return Ok($"Validador {validador.Nome} inserido com sucesso");
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> DeletarValidador(int id)
    {
      try
      {
        DalHelper.DeleteValidador(id);
        return Ok($"Validador {id} deletado com sucesso");
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet()]
    public async Task<ActionResult> RetornarValidadores()
    {
      try
      {
        var validadores = DalHelper.GetValidadores();

        // corrigindo Ip de validadores
        int index = 0;
        foreach (var validador in validadores)
        {
          validadores[index].Ip = validador.Ip.Replace("-", ".");
          index++;
        }

        validadores = validadores.OrderBy(x => x.Stake).ToList();
        validadores.Reverse();

        return Ok(validadores);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("chave/{chave}")]
    public async Task<ActionResult> ComparaChaves(string chave)
    {
      try
      {
        var chaves = DalHelper.GetChaveUnica();
        if (chaves.Contains(chave))
        {
          return Ok(1);
        }
        return Ok(2);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}
