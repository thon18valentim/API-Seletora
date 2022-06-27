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
      // coletando horario
      GerenciadorIntegracao gerenciadorIntegracao = new();
      var horario = "";
      var horarioFormatado = 0f;
      var horarioRequisicao = 0f;
      var horarioRecebimento = 0f;
      try
      {
        DateTime foo = DateTime.Now;
        horarioRequisicao = ((DateTimeOffset)foo).ToUnixTimeSeconds();

        horario = await gerenciadorIntegracao.ObterHorarioGerenciador();
        foo = DateTime.Now;
        horarioRecebimento = ((DateTimeOffset)foo).ToUnixTimeSeconds();

        horarioFormatado = float.Parse(horario);
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
      var tempo = SyncTime.CristianSyncTime(horarioFormatado, horarioRequisicao, horarioRecebimento);

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

        // cadastrando transacao
        DalHelper.AddTransacao(transacao);

        // selecionando validadores
        ValidadorIntegracao validadorIntegracao = new();
        switch (validadoresCount)
        {
          case 1:
            try
            {
              // Não há eleição
              var resposta = await validadorIntegracao.SelecionarValidador($"http://{validadores[0].Ip}/Transacao", transacao);
              transacao.Status = resposta;
              var resultado =  await gerenciadorIntegracao.AtualizarStatusTransacao(transacao);

              return Ok(resultado);
            }
            catch (Exception ex)
            {
              return StatusCode(500, ex.Message);
            }
          case 2:
            return StatusCode(500, "Não há a quantidade correta de validadores para uma eleição, cadastre ao menos 3");
          default:
            try
            {
              List<int> respostas = new();
              respostas.Add(await validadorIntegracao.SelecionarValidador($"http://{validadores[0].Ip}/Transacao", transacao));
              respostas.Add(await validadorIntegracao.SelecionarValidador($"http://{validadores[1].Ip}/Transacao", transacao));
              respostas.Add(await validadorIntegracao.SelecionarValidador($"http://{validadores[2].Ip}/Transacao", transacao));

              var resposta = respostas.FindMostCommon();

              // Validadores offline
              if (resposta == -1)
              {
                transacao.Status = 2;
                await gerenciadorIntegracao.AtualizarStatusTransacao(transacao);
                return Ok(resposta);
              }

              transacao.Status = resposta;
              await gerenciadorIntegracao.AtualizarStatusTransacao(transacao);

              return Ok(resposta);
            }
            catch (Exception ex)
            {
              return StatusCode(500, ex.Message);
            }
        }
      }
      
      return Ok();
    } 
  }
}
