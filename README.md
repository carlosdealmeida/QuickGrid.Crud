# QuickGrid.Crud - Em Construção...

![GitHub last commit](https://img.shields.io/github/last-commit/carlosdealmeida/QuickGrid.Crud)
![GitHub tag (latest SemVer)](https://img.shields.io/github/v/tag/carlosdealmeida/QuickGrid.Crud)
![GitHub](https://img.shields.io/github/license/carlosdealmeida/QuickGrid.Crud)
![GitHub issues](https://img.shields.io/github/issues/carlosdealmeida/QuickGrid.Crud)
![GitHub pull requests](https://img.shields.io/github/issues-pr/carlosdealmeida/QuickGrid.Crud)
![Nuget](https://img.shields.io/nuget/v/QuickGrid.Crud)
![Nuget](https://img.shields.io/nuget/dt/QuickGrid.Crud?label=nuget%20downloads)


E aí, devs? Beleza? Bem-vindos ao `QuickGrid.Crud`, a biblioteca que vai deixar seu projeto mais carioca que praia de Copacabana no verão! Aqui você vai dar aquele upgrade nas suas tabelas com operações de CRUD num piscar de olhos!

## Como Instalar

Manda essa no seu terminal para adicionar a biblioteca ao seu projeto via NuGet:

```bash
dotnet add package QuickGrid.Crud
```

## Como Usar

```csharp
// Primeiro, importe a biblioteca
using QuickGrid.Crud;
```

```csharp
// Depois é só usar o componente QuickGridCrud no seu código
<QuickGridCrud TItem = "ExampleModel" TController = "ExampleController" TEntity = "ExampleEntity" />
```

O QuickGrid.Crud usa o padrão MVC, onde:
TItem - É a nossa Model, ela representa os campos disponíveis para o QuickGrid.Crud.
QuickGridCrud - É a nossa View, ela é renderizada pelo código acima.
TController - É a nossa Controller, lá ficará os métodos que podem ser acessíveis pelo QuickGrid.Crud.

Mas e o TEntity? O TEntity representa nossa entidade na fonte de dados. Ela pode ser uma outra classe ou até mesmo a nossa Model definida em TItem.
Ela fica responsável para montagem da parte do filtro no banco de dados.

Veja nosso QuickGrid.Examples pra pegar aquela inspiração marota e entender como tudo funciona na prática.

## Exemplos
Quer ver a magia acontecendo? Dá um pulo no nosso projeto de exemplo QuickGrid.Examples e veja a QuickGrid.Crud brilhando mais que sol no Arpoador!

## Contribuição
Partiu contribuir? Abre uma issue ou envia um pull request. Ajuda a gente a deixar o QuickGrid.Crud mais irado que final de campeonato no Maracanã!

## Licença
Esse projeto tá no esquema MIT. Pode checar o arquivo LICENSE para mais detalhes.
