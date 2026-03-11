using System.Text.RegularExpressions;

namespace CardValidatorApi;

// A classe card é a classe que representa um cartão de crédito.
// Ela vai possuir as propriedades necessárias para validar o cartão.
// Número do cartão e Bandeira.

public class Card
{
    public string Number { get; }
    public string CardBrand => GetCardBrand();

    public Card(string number)
    {
        Number = NormalizeNumber(number);
    }

    // O método GetCardBrand() deve identificar a bandeira do cartão usando expressões regulares para cada bandeira.
    // Grupo 1: Elo, Hipercard, cartões regionais e Aura (verificados primeiro por terem prefixos
    // que conflitam com bandeiras de maior abrangência como Visa, Mastercard e Discover).
    private static readonly Regex _elo = new(
        @"^(4011(78|79)|43(1274|8935)|45(1416|7393|763[12])|50(4175|6699|67[0-6][0-9]|677[0-8]|9[0-8][0-9]{2}|99[0-8][0-9]|999[0-9])|627780|63(6297|6368|6369)|65(00(3[1-9]|[45][0-9])|04(0[5-9]|[1-3][0-9]|8[5-9]|9[0-9])|05([0-2][0-9]|3[0-8]|4[1-9]|[5-8][0-9]|9[0-8])|07(0[0-9]|1[0-8]|2[0-7])|09(0[1-9]|[1-6][0-9]|7[0-8])|16(5[2-9]|[67][0-9])|50(0[0-9]|1[0-9]|2[1-9]|[34][0-9]|5[0-8])))[0-9]{10}$",
        RegexOptions.Compiled);

    private static readonly Regex _hipercard = new(
        @"^(606282[0-9]{10}|3841[046]0[0-9]{13})$",
        RegexOptions.Compiled);

    private static readonly Regex _banaseCard = new(
        @"^636117[0-9]{10}$",
        RegexOptions.Compiled);

    private static readonly Regex _fortBrasil = new(
        @"^628167[0-9]{10}$",
        RegexOptions.Compiled);

    private static readonly Regex _grandCard = new(
        @"^605032[0-9]{10}$",
        RegexOptions.Compiled);

    private static readonly Regex _personalCard = new(
        @"^636085[0-9]{10}$",
        RegexOptions.Compiled);

    private static readonly Regex _sorocred = new(
        @"^(627892|636414)[0-9]{10}$",
        RegexOptions.Compiled);

    private static readonly Regex _cabal = new(
        @"^(60420[1-9]|6042[1-9][0-9]|6043[0-9]{2}|604400)[0-9]{10}$",
        RegexOptions.Compiled);

    private static readonly Regex _valecard = new(
        @"^6064(?:44|58|82)[0-9]{10}$",
        RegexOptions.Compiled);

    private static readonly Regex _aura = new(
        @"^50[0-9]{14,17}$",
        RegexOptions.Compiled);

    // Grupo 2: Discover, Diners Club e JCB.
    private static readonly Regex _discover = new(
        @"^6(?:011|5[0-9]{2}|4[4-9][0-9])[0-9]{12}$",
        RegexOptions.Compiled);

    private static readonly Regex _dinersClub = new(
        @"^3(?:0[0-5]|[68][0-9])[0-9]{11}$",
        RegexOptions.Compiled);

    private static readonly Regex _jcb = new(
        @"^(?:2131|1800|35[0-9]{3})[0-9]{11}$",
        RegexOptions.Compiled);

    // Grupo 3: Amex.
    private static readonly Regex _amex = new(
        @"^3[47][0-9]{13}$",
        RegexOptions.Compiled);

    // Grupo 4: Mastercard (série 51-55 e nova série 2221-2720).
    private static readonly Regex _mastercard = new(
        @"^(5[1-5][0-9]{14}|2(22[1-9][0-9]{12}|2[3-9][0-9]{13}|[3-6][0-9]{14}|7[0-1][0-9]{13}|720[0-9]{12}))$",
        RegexOptions.Compiled);

    // Grupo 5: Visa (13 ou 16 dígitos).
    private static readonly Regex _visa = new(
        @"^4[0-9]{12}(?:[0-9]{3})?$",
        RegexOptions.Compiled);

    private string GetCardBrand()
    {
        if (string.IsNullOrEmpty(Number))
            return "Bandeira não identificada ou cartão inválido.";

        // 1- Elo, Hipercard, cartões regionais e Aura.
        if (_elo.IsMatch(Number)) return "Elo";
        if (_hipercard.IsMatch(Number)) return "Hipercard";
        if (_banaseCard.IsMatch(Number)) return "Banese Card";
        if (_fortBrasil.IsMatch(Number)) return "Fort Brasil";
        if (_grandCard.IsMatch(Number)) return "GrandCard";
        if (_personalCard.IsMatch(Number)) return "Personal Card";
        if (_sorocred.IsMatch(Number)) return "Sorocred";
        if (_cabal.IsMatch(Number)) return "Cabal";
        if (_valecard.IsMatch(Number)) return "Valecard";
        if (_aura.IsMatch(Number)) return "Aura";

        // 2- Discover, Diners Club e JCB.
        if (_discover.IsMatch(Number)) return "Discover";
        if (_dinersClub.IsMatch(Number)) return "Diners Club";
        if (_jcb.IsMatch(Number)) return "JCB";

        // 3- Amex.
        if (_amex.IsMatch(Number)) return "Amex";

        // 4- Mastercard.
        if (_mastercard.IsMatch(Number)) return "Mastercard";

        // 5- Visa.
        if (_visa.IsMatch(Number)) return "Visa";

        return "Bandeira não identificada ou cartão inválido.";
    }

    // Crie um metodo privado para normalizar o número do cartão, removendo espaços e traços, deixando apenas os dígitos.
    private static string NormalizeNumber(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        char[] digits = new char[value.Length];
        int index = 0;

        foreach (char c in value)
            if (char.IsDigit(c))
                digits[index++] = c;

        return new string(digits, 0, index);
    }

    // Agora o método ValidateCardNumber() vai usar o algoritmo de Luhn para validar o número do cartão.
    public bool ValidateCardNumber()
    {
        if (string.IsNullOrEmpty(Number))
            return false;

        int sum = 0;
        bool isEven = false;

        for (int i = Number.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(Number[i].ToString());

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
}