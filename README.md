# API Seletora de Validadores - Universidade Positivo
API para seleção de validadores de transações.

## Envolvidos no Projeto
- Bernardo Carneiro
- Heitor Hellou
- Othon Valentim

## Sobre a aplicação
A aplicação foi desenvolvida para receber transações financeiras, de uma rede local blockchain simulada, de um gerenciador e selecionar Validadores para a transação em questão. Além de selecionar os Validadores da transação recebida a aplicação eleje a resposta de transação aprovada ou não para enfim encaminhar para o gerenciador o sucesso ou fracasso da aplicação.

## Arquitetura
Projeto com arquitetura baseada em:
- .NET Core 6
- SQLite
- Clean Architecture

## Como executar o projeto
Rode o projeto de preferência no Visual Studio 2022. Você pode utilizar o próprio swagger para enviar as requisições ou o Postman.

## UseCases
Para iniciar o fluxo do seletor, é crucial que haja validadores cadastrados no sistema. Para isso, envie para a rota POST abaixo um objeto json com os seguintes dados:

`/Validador`

```
{
  "id": 0,
  "nome": "string",
  "ip": "string",
  "stake": 0
}
```

Após cadastrar os validadores podemos enviar a nossa primeira transação. Utilizando a rota POST abaixo devemos enviar um objeto json do tipo transação para o sistema:

`/Transacao`

```
{
  "id": 0,
  "remetente": 0,
  "recebedor": 0,
  "valor": 0,
  "horario": "string",
  "status": 0
}
```

## Fluxo da aplicação
- Cadastra validadores
- Recebe transação
- Sincroniza tempo com o gerenciador
- Valida campos básicos da transação
- Seleciona validadores por stake
- Envia transação para validadores
- Eleje o resultado da transação
- Atualiza transaçõ no gerenciador

## Rotas adicionais
Para consultar as transações armazenadas no banco basta chamar a rota GET abaixo:

`/Transacao`

Para transações específicas envie o id da transação requerida:

`/Transacao/{id}`

Para consultar os validadores cadastrados no sistema basta chamar a rota GET abaixo:

`/Validador`

Para deletar um validador basta consumir a rota DELETE abaixo enviando o id:

`/Validador/delete/{id}`

## Observações
É importante ressaltar que o horário enviado para o sistema sempre deve estar convertido em unixtime, para que possamos globalizar o envio e recebimento do horário das transações.

O ip que será cadastrado junto ao validador deve estar no seguinte modelo: 0-0-0-0:0000

Por conta de limitações do banco SQLite é crucial que o modelo seja seguido. No momento das requisições os IPs serão corrigidos pelo sistema, dessa maneira não causando problemas em nenhuma das pontas.

A escolha de validadores será sempre baseada no valor do stake, que pode ser alterado dependendo da resposta do validador.
