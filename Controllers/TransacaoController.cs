using Microsoft.AspNetCore.Mvc;
using Transacoes_blockchain.infra.api;
using Transacoes_blockchain.infra.data;
using Transacoes_blockchain.utils;

namespace Transacoes_blockchain.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class TransacaoController: ControllerBase
  {
    [HttpGet]
    public async Task<ActionResult<Transacao>> RetornarTransacoes()
    {
      var transacoes = DalHelper.GetTransacoes();

      return Ok(transacoes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Transacao>> GetTransacao(int id)
    {
      var transacao = DalHelper.GetById("Transacoes", id);
      return Ok(transacao);
    }

    [HttpPost]
    public async Task<ActionResult> InserirTransacao(Transacao transacao)
    {
      try
      {
        GerenciadorIntegracao integra = new();
        var horario = await integra.ObterHorarioGerenciador();
      }
      catch
      {
        return StatusCode(500, "Nao foi possivel obter o horario do gerenciador");
      }

      // validacao basica da transacao antes de prosseguir
      if (!transacao.Validate())
      {
        return BadRequest($"Transacao {transacao.Id} possui status invalido");
      }

      // sync time

      if(transacao.Status == 0)
      {
        // carregando validadores cadastrados
        var validadores = DalHelper.GetValidadores();
        var validadoresCount = validadores.Count;
        if (validadoresCount == 0)
        {
          return StatusCode(500, "Nao há validadores cadastrados para prosseguir com a transacao");
        }

        // corrigindo Ip de validadores
        int index = 0;
        foreach(var validador in validadores)
        {
          validadores[index].Ip = validador.Ip.Replace("-", ".");
          index++;
        }

        // selecionando validadores
        List<Validador> selecionados = new();
        switch (validadoresCount)
        {
          case 1:
            selecionados.Add(validadores[0]);
            break;
          case 2:
            selecionados.Add(validadores[0]);
            selecionados.Add(validadores[1]);
            break;
          default:
            selecionados.Add(validadores[0]);
            selecionados.Add(validadores[1]);
            selecionados.Add(validadores[2]);
            break;
        }

        // cadastrando transacao
        DalHelper.AddTransacao(transacao);

        return Ok(selecionados);
      }
      
      return Ok();
    }
  }
}
