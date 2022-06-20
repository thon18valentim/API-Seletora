using Microsoft.AspNetCore.Mvc;
using Transacoes_blockchain.infra.api;
using Transacoes_blockchain.infra.data;

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
        DalHelper.AddValidador(validador);
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
        return Ok(validadores);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}
