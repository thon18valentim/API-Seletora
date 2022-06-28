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
      var transacao = DalHelper.GetTransacaoById(id);
      return Ok(transacao);
    }

    [HttpPost]
    public async Task<ActionResult> InserirTransacao(Transacao transacao)
    {
      // verificando se cliente não tem punição
      DateTime horarioSistema = DateTime.Now;
      var punicaoAtiva = DalHelper.VerificarPunicao(transacao.Remetente);
      if (punicaoAtiva.Ativa == 1)
      {
        var tempoPunicao = ((DateTimeOffset)horarioSistema).ToUnixTimeSeconds() - float.Parse(punicaoAtiva.Inicio);
        return StatusCode(423, $"Cliente {punicaoAtiva.Id} tem uma punição ativa");
      }

      // coletando horario
      GerenciadorIntegracao gerenciadorIntegracao = new();
      var horario = "";
      var horarioFormatado = 0f;
      var horarioRequisicao = 0f;
      var horarioRecebimento = 0f;
      try
      {
        horarioRequisicao = ((DateTimeOffset)horarioSistema).ToUnixTimeSeconds();

        horario = await gerenciadorIntegracao.ObterHorarioGerenciador();
        horarioSistema = DateTime.Now;
        horarioRecebimento = ((DateTimeOffset)horarioSistema).ToUnixTimeSeconds();

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

      if (transacao.Status == 0)
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
              var resposta = await validadorIntegracao.SelecionarValidador($"https://{validadores[0].Ip}/Transacao", transacao);
              transacao.Status = resposta;
              var resultado =  await gerenciadorIntegracao.AtualizarStatusTransacao(transacao);

              // Aplicar punição a cliente
              var punicao = DalHelper.VerificarPunicao(transacao.Remetente);
              if (resposta == 2)
              {
                // Incrementando erros
                punicao.Erros++;
                if (punicao.Id != 0)
                {
                  if (punicao.Erros > 3)
                  {
                    // punindo cliente se punicao não estiver ativa
                    // se estiver ativa e cliente continua errando continue ampliando a punicao
                    DalHelper.UpdatePunicao(transacao.Remetente, punicao.Erros, 1);
                  }
                  else
                  {
                    // atualiza punicao com o numero de erros, vale ressaltar que esse 
                    // cliente ainda não é elegível a punição
                    DalHelper.UpdatePunicao(transacao.Remetente, punicao.Erros, 0);
                  }
                }
                else
                {
                  // Iniciando contagem de erros
                  DalHelper.AddPunicao(transacao.Remetente);
                }
              }
              else
              {
                if (punicao.Ativa == 1)
                {
                  // se estiver ativa verificamos quanto tempo falta
                  var inicioDaPunicao = float.Parse(punicao.Inicio);
                  DateTime foo = DateTime.Now;
                  var horarioAtual = ((DateTimeOffset)foo).ToUnixTimeSeconds();

                  // retirando punicao
                  if (horarioAtual - inicioDaPunicao >= 300)
                  {
                    DalHelper.UpdatePunicao(transacao.Remetente, 0, 0);
                  }
                }
              }

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
              respostas.Add(await validadorIntegracao.SelecionarValidador($"https://{validadores[0].Ip}/Transacao", transacao));
              respostas.Add(await validadorIntegracao.SelecionarValidador($"https://{validadores[1].Ip}/Transacao", transacao));
              respostas.Add(await validadorIntegracao.SelecionarValidador($"https://{validadores[2].Ip}/Transacao", transacao));

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

              // Aplicando punicoes em Validadores
              if(respostas[0] != resposta)
              {
                validadores[0].Stake -= 3;
                if(validadores[0].Stake < 0)
                {
                  validadores[0].Stake = 0;
                }
                DalHelper.UpdateValidador(validadores[0]);
              }
              if (respostas[1] != resposta)
              {
                validadores[1].Stake -= 3;
                if (validadores[1].Stake < 0)
                {
                  validadores[1].Stake = 0;
                }
                DalHelper.UpdateValidador(validadores[1]);
              }
              if (respostas[2] != resposta)
              {
                validadores[2].Stake -= 3;
                if (validadores[2].Stake < 0)
                {
                  validadores[2].Stake = 0;
                }
                DalHelper.UpdateValidador(validadores[2]);
              }

              // Aplicar premiação aos Validadores
              if (respostas[0] == resposta)
              {
                validadores[0].Stake += 5;
                if (validadores[0].Stake > 50)
                {
                  validadores[0].Stake = 50;
                }
                DalHelper.UpdateValidador(validadores[0]);
              }
              if (respostas[1] == resposta)
              {
                validadores[1].Stake += 5;
                if (validadores[1].Stake > 50)
                {
                  validadores[1].Stake = 50;
                }
                DalHelper.UpdateValidador(validadores[1]);
              }
              if (respostas[2] == resposta)
              {
                validadores[2].Stake += 5;
                if (validadores[2].Stake > 50)
                {
                  validadores[2].Stake = 50;
                }
                DalHelper.UpdateValidador(validadores[2]);
              }

              // Aplicar punição a cliente
              var punicao = DalHelper.VerificarPunicao(transacao.Remetente);
              if (resposta == 2)
              {
                // Incrementando erros
                punicao.Erros++;
                if (punicao.Id != 0)
                {
                  if(punicao.Erros > 3)
                  {
                    // punindo cliente se punicao não estiver ativa
                    // se estiver ativa e cliente continua errando continue ampliando a punicao
                    DalHelper.UpdatePunicao(transacao.Remetente, punicao.Erros, 1);
                  }
                  else
                  {
                    // atualiza punicao com o numero de erros, vale ressaltar que esse 
                    // cliente ainda não é elegível a punição
                    DalHelper.UpdatePunicao(transacao.Remetente, punicao.Erros, 0);
                  }
                }
                else
                {
                  // Iniciando contagem de erros
                  DalHelper.AddPunicao(transacao.Remetente);
                }
              }
              else
              {
                if(punicao.Ativa == 1)
                {
                  // se estiver ativa verificamos quanto tempo falta
                  var inicioDaPunicao = float.Parse(punicao.Inicio);
                  DateTime foo = DateTime.Now;
                  var horarioAtual = ((DateTimeOffset)foo).ToUnixTimeSeconds();

                  // retirando punicao
                  if (horarioAtual - inicioDaPunicao >= 300)
                  {
                    DalHelper.UpdatePunicao(transacao.Remetente, 0, 0);
                  }
                }
              }

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
