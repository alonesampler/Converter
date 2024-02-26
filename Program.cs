using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Converter
{
    public class Program
    {
        static string from = "";
        static string to = "";
        static double numberFromChoice = 0;
        static string apiKey = "1dec2ed8ba5a45e68b4987e246788e4b";

        static async Task Main()
        {
            bool cont = true;

            while (cont)
            {
                Сhoice();
                Converter();

                double rate = await GetExchangeRate(from, to);
                double convertedAmount = numberFromChoice * rate;

                Console.WriteLine($"Результат конвертации: {convertedAmount} {to}");

                Console.WriteLine("Continue? y / n");
                string yesOrNo = Console.ReadLine();
                if (yesOrNo == "n") cont = false;
            }
        }

        public static void Сhoice()
        {
            Console.WriteLine("Выбери из какой валюты конвертировать.");
            Console.WriteLine("USD, RUB, EUR, KGS, KZT");
            Console.WriteLine("Примечание нужно писать как принято большими буквами");
            from = Console.ReadLine();

            Console.WriteLine("Выбери в какую валюту конвертировать.");
            Console.WriteLine("USD, RUB, EUR, KGS, KZT");
            Console.WriteLine("Примечание нужно писать как принято большими буквами");
            to = Console.ReadLine();
        }

        public static void Converter()
        {
            Console.WriteLine("Сколько конвертировать?");
            numberFromChoice = double.Parse(Console.ReadLine());
        }

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