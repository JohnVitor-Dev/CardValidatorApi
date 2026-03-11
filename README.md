# CardValidatorApi 💳

O objetivo é criar uma API capaz de identificar a bandeira de um cartão de crédito e validar seu número, explorando o uso do **GitHub Copilot** como assistente de codificação durante todo o desenvolvimento.

---
## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [O que é GitHub Copilot?](#o-que-é-github-copilot)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Como Funciona](#como-funciona)
  - [Identificação da Bandeira](#identificação-da-bandeira)
  - [Algoritmo de Luhn](#algoritmo-de-luhn)
- [Endpoints da API](#endpoints-da-api)
- [Bandeiras Suportadas](#bandeiras-suportadas)
- [Conceitos Abordados](#conceitos-abordados)

---

## Sobre o Projeto

O **CardValidatorApi** é uma API REST construída com **ASP.NET Core 6**, que recebe o número de um cartão de crédito e retorna duas informações:

1. **A bandeira do cartão** (Visa, Mastercard, Elo, Amex, etc.)
2. **Se o número do cartão é válido** (usando o algoritmo de Luhn)

A ideia surgiu como desafio prático para unir conhecimentos de C#, APIs e boas práticas de desenvolvimento, com o diferencial de usar o **GitHub Copilot** como parceiro durante a escrita do código.

---

## O que é GitHub Copilot?

O **GitHub Copilot** é uma ferramenta de IA desenvolvida pela GitHub em parceria com a OpenAI. Ele funciona como um assistente de programação diretamente no seu editor de código, sugerindo linhas, funções e até blocos inteiros de código com base no contexto do que você está escrevendo.

Durante o desenvolvimento deste projeto, o Copilot foi usado para:

- Sugerir as expressões regulares (regex) para identificar cada bandeira
- Auxiliar na implementação do algoritmo de Luhn
- Completar automaticamente código repetitivo (como os padrões de cada bandeira)
- Ajudar a estruturar o controller da API

> O Copilot não substitui o programador — ele acelera o trabalho e sugere soluções que você ainda precisa entender, revisar e adaptar.

---

## Tecnologias Utilizadas

| Tecnologia | Versão | Finalidade |
|---|---|---|
| [.NET](https://dotnet.microsoft.com/) | 6.0 | Framework principal |
| [ASP.NET Core](https://learn.microsoft.com/aspnet/core) | 6.0 | Criação da API REST |
| [Swashbuckle / Swagger](https://swagger.io/) | 6.5.0 | Documentação e teste da API |
| [GitHub Copilot](https://github.com/features/copilot) | — | Assistente de IA para codificação |
| C# | 10 | Linguagem de programação |

---

## Estrutura do Projeto

```
CardValidatorApi/
├── Controllers/
│   └── CardValidatorController.cs   # Recebe as requisições HTTP e chama a lógica de validação
├── Models/
|   └── Card.cs                      # Classe principal: identifica bandeira e validação
├── Properties/
│   └── launchSettings.json          
├── Program.cs                       # Ponto de entrada da aplicação, 
├── CardValidatorApi.csproj          
├── appsettings.json                 
└── appsettings.Development.json     
```

### Responsabilidade de cada arquivo

- **`Card.cs`** — É o coração do projeto. Contém toda a lógica de negócio: as expressões regulares para cada bandeira e o algoritmo de Luhn para validação.
- **`CardValidatorController.cs`** — Camada de entrada HTTP. Recebe o número do cartão, cria um objeto `Card` e retorna o resultado.
- **`Program.cs`** — Configura os serviços da aplicação, incluindo o Swagger para facilitar os testes.

---

## Como Funciona

### Identificação da Bandeira

Cada bandeira de cartão segue um padrão numérico diferente. Por exemplo:

- Cartões **Visa** sempre começam com `4`
- Cartões **Mastercard** começam com `51` a `55` (ou `2221` a `2720` na série mais nova)
- Cartões **Amex** começam com `34` ou `37`

O projeto usa **expressões regulares (Regex)** para mapear esses padrões. As bandeiras são verificadas em grupos, da mais específica para a mais genérica, evitando conflitos:

**Grupo 1 — Bandeiras regionais e nacionais** (verificadas primeiro por ter prefixos que se sobrepõem aos de bandeiras internacionais):
- Elo, Hipercard, Banese Card, Fort Brasil, GrandCard, Personal Card, Sorocred, Cabal, Valecard, Aura

**Grupo 2 — Bandeiras internacionais de nicho:**
- Discover, Diners Club, JCB

**Grupo 3 — American Express:**
- Amex

**Grupo 4 — Mastercard**

**Grupo 5 — Visa** (verificada por último, pois tem o padrão mais amplo)

```csharp
// Exemplo do regex da Visa — começa com 4, com 13 ou 16 dígitos no total
private static readonly Regex _visa = new(
    @"^4[0-9]{12}(?:[0-9]{3})?$",
    RegexOptions.Compiled);
```

Os objetos `Regex` são declarados como `static readonly` para serem compilados uma única vez e reutilizados em todas as requisições, o que melhora a performance.

> A maior parte das expressões regulares utilizadas para identificação das bandeiras foi baseada no gist [iin_card](https://gist.github.com/gusribeiro/263a165db774f5d78251#file-iin_card) de [@gusribeiro](https://github.com/gusribeiro).

---

### Algoritmo de Luhn

O **algoritmo de Luhn** (também chamado de "fórmula mod 10") é um método matemático simples usado para verificar se um número de cartão de crédito é válido. Ele não garante que o cartão existe, só que o número segue uma estrutura correta.

**Como funciona, passo a passo:**

1. Percorre os dígitos do número **da direita para a esquerda**
2. Os dígitos em posições pares (contando da direita, base 1) são **dobrados**
3. Se o resultado do dobramento for maior que 9, **subtrai-se 9**
4. Soma todos os dígitos
5. Se a soma for divisível por 10, o cartão é **válido**

```csharp
public bool ValidateCardNumber()
{
    string number = NormalizeNumber(Number);
    int sum = 0;
    bool isEven = false;

    for (int i = number.Length - 1; i >= 0; i--)
    {
        int digit = int.Parse(number[i].ToString());

        if (isEven)
        {
            digit *= 2;
            if (digit > 9)
                digit -= 9;
        }

        sum += digit;
        isEven = !isEven;
    }

    return sum % 10 == 0;
}
```

> O método `NormalizeNumber()` remove espaços, traços e outros caracteres antes da validação, então você pode passar `4111 1111 1111 1111` ou `4111-1111-1111-1111` que funciona normalmente.

---

## Endpoints da API

### `GET /CardValidator`

Valida um número de cartão e identifica sua bandeira.

**Parâmetros:**

| Parâmetro | Tipo | Obrigatório | Descrição |
|---|---|---|---|
| `number` | `string` | Sim | Número do cartão (pode conter espaços ou traços) |

---

**Exemplo de resposta para um Visa válido:**
```json
{
  "cardBrand": "Visa",
  "isValid": true
}
```

**Exemplo de resposta para número inválido:**
```json
{
  "cardBrand": "Bandeira não identificada ou cartão inválido.",
  "isValid": false
}
```

> **Nota:** Os números de cartão usados nos exemplos são números de **teste** amplamente conhecidos e não representam cartões reais.

---

## Bandeiras Suportadas

| Bandeira | Abrangência |
|---|---|
| Visa | Internacional |
| Mastercard | Internacional |
| American Express (Amex) | Internacional |
| Discover | Internacional |
| Diners Club | Internacional |
| JCB | Internacional |
| Elo | Brasil |
| Hipercard | Brasil |
| Aura | Brasil |
| Banese Card | Brasil (regional) |
| Fort Brasil | Brasil (regional) |
| GrandCard | Brasil (regional) |
| Personal Card | Brasil (regional) |
| Sorocred | Brasil (regional) |
| Cabal | Brasil (regional) |
| Valecard | Brasil (regional) |

---

## Conceitos Abordados

### 🔹 Introdução ao Ambiente .NET
O projeto roda em **.NET 6**, um dos frameworks mais utilizados para desenvolvimento de aplicações modernas. O arquivo `.csproj` define o target framework e as dependências do projeto.

### 🔹 Sintaxe Básica com .NET C#
Toda a lógica do projeto é escrita em **C#**: declaração de classes, propriedades, construtores, laços `for`, condicionais `if`, operadores e o uso de `string`, `int`, `bool` e arrays de caracteres.

### 🔹 Dados e Listas com .NET C#
O método `NormalizeNumber()` usa um **array de char** para construir o número normalizado de forma eficiente, evitando concatenações de string desnecessárias.

### 🔹 Programação Orientada a Objetos com C#
A classe `Card` encapsula todo o comportamento relacionado ao cartão:
- **Encapsulamento:** `CardBrand` é somente leitura (`get` público, sem `set`)
- **Construtor:** inicializa o número e já calcula a bandeira
- **Métodos privados:** `cardBrand()` e `NormalizeNumber()` são detalhes internos da classe
- **Membros estáticos:** os `Regex` são `static readonly` para serem compartilhados entre instâncias

### 🔹 Criando APIs com .NET C#
A API foi construída com **ASP.NET Core**:
- `[ApiController]` e `[Route]` configuram o roteamento automaticamente
- `IActionResult` e `Ok()` retornam respostas HTTP padronizadas
- O **Swagger** (Swashbuckle) é configurado no `Program.cs` para documentação automática

### 🔹 Prompt Engineering para GitHub Copilot
Durante o desenvolvimento, foram usadas técnicas de **prompt engineering** para obter melhores sugestões do Copilot:
- Comentários descritivos antes dos métodos para guiar as sugestões
- Nomes de variáveis e métodos claros e semânticos
- Comentários explicando o contexto e a intenção do código

---

## Autor

Desenvolvido como desafio final do Bootcamp **TIVIT - .NET com GitHub Copilot** na [DIO](https://www.dio.me/).
