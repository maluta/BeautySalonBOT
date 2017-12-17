using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using WeatherBot.Model;

namespace WeatherBot.Services
{
    public class WeatherService
    {
        private static string APIKey = "5e46e301d42528287fe3a46262e7a99b";
        private static string APILanguage = "pt";

        public async static Task<OpenWeather> GetForescast(string city)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://api.openweathermap.org/");
                HttpResponseMessage response = client.GetAsync($"data/2.5/weather?q={RemoveAccents(city)}&APPID={APIKey}&units=metric&lang={APILanguage}").Result;

                var responseJson = await response.Content.ReadAsStringAsync();

                OpenWeather result = JsonConvert.DeserializeObject<OpenWeather>(responseJson);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string RemoveAccents(string text)
        {
            return new string(text
                .Normalize(NormalizationForm.FormD)
                .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }
    }
}