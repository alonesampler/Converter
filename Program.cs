using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Converter
{
    public class Program
    {
        static string fromCurrency = "";
        static string toCurrency = "";
        static double amountToConvert = 0;
        static string apiKey = "1dec2ed8ba5a45e68b4987e246788e4b";

        static async Task Main()
        {
            bool cont = true;

            while (cont)
            {
                CurrencyChoice choice = new CurrencyChoice();
                choice.Choose();
                Converter converter = new Converter();
                converter.Convert();

                double rate = await CurrencySelection.GetExchangeRate(fromCurrency, toCurrency);
                double convertedAmount = amountToConvert * rate;

                Console.WriteLine($"Результат конвертации: {convertedAmount} {toCurrency}");

                Console.WriteLine("Продолжить? y / n");
                string yesOrNo = Console.ReadLine();
                if (yesOrNo == "n") cont = false;
            }
        }
    }

    public interface IChoice
    {
        void Choose();
    }

    public class CurrencyChoice : IChoice
    {
        public void Choose()
        {
            Console.WriteLine("Выберите валюту для конвертации.");
            Console.WriteLine("USD, RUB, EUR, KGS, KZT");
            Console.WriteLine("Примечание: введите заглавными буквами");
            fromCurrency = Console.ReadLine();

            Console.WriteLine("Выберите валюту для конвертации.");
            Console.WriteLine("USD, RUB, EUR, KGS, KZT");
            Console.WriteLine("Примечание: введите заглавными буквами");
            toCurrency = Console.ReadLine();
        }
    }

    public interface IConverter
    {
        void Convert();
    }

    public class Converter : IConverter
    {
        public void Convert()
        {
            Console.WriteLine("Сколько вы хотите конвертировать?");
            amountToConvert = double.Parse(Console.ReadLine());
        }
    }

    public class CurrencySelection
    {
        public static async Task<double> GetExchangeRate(string fromCurrency, string toCurrency)
        {
            string url = $"https://open.er-api.com/v6/latest/{fromCurrency}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Token {apiKey}");
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var json = System.Text.Json.JsonDocument.Parse(responseBody);
                    var rates = json.RootElement.GetProperty("rates");
                    double rate = rates.GetProperty(toCurrency).GetDouble();
                    return rate;
                }
                else
                {
                    Console.WriteLine($"Ошибка при получении курса обмена: {response.StatusCode}");
                    return 0;
                }
            }
        }
    }
}
