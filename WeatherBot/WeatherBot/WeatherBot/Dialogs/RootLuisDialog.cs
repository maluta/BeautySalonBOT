using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Linq;
using WeatherBot.Services;

namespace WeatherBot.Dialogs
{
    //[LuisModel("appId", "key")]
    [LuisModel("36ae9d02-546c-46f9-868f-625cf9ab3859", "a6875a3811c547d0840f0a459f4f938a")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            // Mensagem que o bot vai devolver ao usuário
            string message = "Não entendi o que você quis dizer.";

            // Avisa ao contexto do bot para ele mandar a mensagem para o usuário via POST
            await context.PostAsync(message);

            // Avisa ao contexto do bot para aguardar uma mensagem de resposta do usuário 
            context.Wait(this.MessageReceived);
        }
                
        [LuisIntent("Wheather_Intent")]
        public async Task WeatherForecast(IDialogContext context, LuisResult result)
        {
            // Obtém o nome da cidade
            var city = result.Entities.FirstOrDefault()?.Entity;

            // Consulta a API com o nome da cidade
            var forecast = await WeatherService.GetForescast(city);

            // Mensagem que o bot vai devolver ao usuário
            string message = $"Tempo em {city.First().ToString().ToUpper() + city.Substring(1)} é de {forecast?.weather?.FirstOrDefault().description} com mínima de {forecast?.main?.temp_min}º e máxima de {forecast?.main?.temp_max}º";

            // Avisa ao contexto do bot para ele mandar a mensagem para o usuário via POST
            await context.PostAsync(message);

            // Avisa ao contexto do bot para aguardar uma mensagem de resposta do usuário 
            context.Wait(this.MessageReceived);
        }
    }
}