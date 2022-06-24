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
      var horario = "";
      try
      {
        GerenciadorIntegracao integra = new();
        horario = await integra.ObterHorarioGerenciador();
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
      SyncTime.CristianSyncTime(horario);

      // Recebendo transacoes como seletor
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

        validadores = validadores.OrderBy(x => x.Stake).ToList();
        validadores.Reverse();

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

        // setando transacao para 'Em Processamento'
        transacao.Status = 3;

        // cadastrando transacao
        DalHelper.AddTransacao(transacao);

        //List<string> responseList = new();
        //try
        //{
        //  ValidadorIntegracao validadorIntegracao = new();

        //  foreach (var validador in selecionados)
        //  {
        //    var response = await validadorIntegracao.SelecionarValidador($"http://{validador.Ip}/Transacao");
        //    responseList.Add(response);
        //  }
        //}
        //catch (Exception ex)
        //{
        //  return StatusCode(500, ex.Message);
        //}

        return Ok(selecionados);
      }

      // recebendo transacao como validador
      if(transacao.Status == 3)
      {

      }
      
      return Ok();
    }

    [HttpPost("/resultado")]
    public async Task<ActionResult> InserirResultadoTransacao(Transacao transacao)
    {
      // fazer eleicao
      if(transacao.Status == 1)
      {

      }
      else
      {

      }

      return Ok();
    } 
  }
}
