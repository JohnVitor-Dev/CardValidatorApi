/* Este controller é responsavel por lidar com as requisições de /CardValidator.
O objetivo é receber o número do cartão, validar e retornar o resultado da validação.
Para isso, ele vai pegar o número do cartão da requisição e criar um objeto Card.
O objeto Card vai ser responsável por validar o número do cartão e identificar a bandeira do cartão.
A validação do número do cartão vai ser feita usando o algoritmo de Luhn.
*/

using Microsoft.AspNetCore.Mvc;

namespace CardValidatorApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardValidatorController : ControllerBase
    {
        // Rota para validar o cartão de crédito e identificar a bandeira do cartão.
        [HttpGet]
        public IActionResult ValidateCard(string number)
        {
            Card card = new Card(number);
            bool isValid = card.ValidateCardNumber();
            string cardBrand = card.CardBrand;
            return Ok(new { CardBrand = cardBrand, IsValid = isValid });
        }

    }
}