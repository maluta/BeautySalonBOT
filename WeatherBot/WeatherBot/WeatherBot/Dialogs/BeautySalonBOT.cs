using System;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using WeatherBot.Comum;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.ConnectorEx;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace WeatherBot.Dialogs
{
    [LuisModel("48bc1681-aa8c-4df5-96e3-3ed2e3b8c997", "6dd8ade7b2844307aa91e6d4d6815221")]
    [Serializable]
    public class BeautySalonBOT : LuisDialog<object>
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

        [LuisIntent("Pedido_Intent")]
        public async Task Pedido(IDialogContext context, LuisResult result)
        {
            string message = string.Empty;

            // Obtém o nome da cidade
            var pedido = result.Entities.ToList(); 

            if (pedido != null)
            {
                if (pedido.Where(ped => ped.Type.ToUpper().Equals(Constantes.TYPE_ATIVIDADE)).FirstOrDefault().Entity.ToUpper().Equals(Constantes.ENTITY_CABELO) &&
                    pedido.Where(ped => ped.Type.ToUpper().Equals(Constantes.TYPE_DATA)).FirstOrDefault().Entity.ToUpper().Equals(Constantes.ENTITY_HOJE))
                {
                    string[] arrayDatas = new string[3];
                    arrayDatas.SetValue(string.Format("{0} as {1}", RetornarData().ToShortDateString(), "15h00"), 0);
                    arrayDatas.SetValue(string.Format("{0} as {1}", RetornarData().ToShortDateString(), "16h00"), 1);
                    arrayDatas.SetValue(string.Format("{0} as {1}", RetornarData().ToShortDateString(), "18h00"), 2);

                    var datas = arrayDatas.OrderBy(c => c.ToString());

                    PromptDialog.Choice(
                           context,
                           PedidoConfirmAsync,
                           datas,
                           "A agenda é:",
                           promptStyle: PromptStyle.Auto);
                }

                else if ((pedido.Any(w => w.Entity.ToUpper().Equals("PÉ") || w.Entity.ToUpper().Equals("MÃO")) && (pedido.Any(w1 => w1.Entity.ToUpper().Equals("HOJE")))))
                {
                    string[] arrayDatas = new string[3];
                    arrayDatas.SetValue(string.Format("Horário disponivel as {0}", "12h00"), 0);
                    arrayDatas.SetValue(string.Format("Horário disponivel as {0}", "13h00"), 1);
                    arrayDatas.SetValue(string.Format("Horário disponivel as {0}", "14h00"), 2);

                    var datas = arrayDatas.OrderBy(c => c.ToString());

                    PromptDialog.Choice(
                           context,
                           PedidoConfirmAsync,
                           datas,
                           "O horário são:",
                           promptStyle: PromptStyle.Auto);
                }
                else if ((pedido.Any(a => a.Entity.ToUpper().Equals("MÃO") || a.Entity.ToUpper().Equals("PÉ")) && (pedido.Any(w1 => w1.Entity.ToUpper().Equals("AMANHÃ")))))
                {
                    string[] arrayDatas = new string[3];
                    arrayDatas.SetValue(string.Format("{0} as {1}", RetornarData().ToShortDateString(), "11h00"), 0);
                    arrayDatas.SetValue(string.Format("{0} as {1}", RetornarData().ToShortDateString(), "17h00"), 1);
                    arrayDatas.SetValue(string.Format("{0} as {1}", RetornarData().ToShortDateString(), "20h00"), 2);

                    var datas = arrayDatas.OrderBy(c => c.ToString());

                    PromptDialog.Choice(
                           context,
                           PedidoConfirmAsync,
                           datas.ToList(),
                           "A agenda é:",
                           promptStyle: PromptStyle.Auto);
                }
            }
            else
            {
                message = "Não consegui entender a sua pergunta.";

                // Avisa ao contexto do bot para ele mandar a mensagem para o usuário via POST
                await context.PostAsync(message);

                // Avisa ao contexto do bot para aguardar uma mensagem de resposta do usuário 
                context.Wait(this.MessageReceived);
            }
        }

        public async Task PedidoConfirmAsync(IDialogContext context, IAwaitable<string> result)
        {
            var Prompt = await result;
            string PromptString = Prompt.ToString().ToLower().Trim();
            if (PromptString.Length > 0)
            {
                var message = $"Confirmado para o dia {Prompt}";

                await context.PostAsync(message);
            }
            else
                await context.PostAsync("Ok, mande outra coisa.");

            context.Wait(MessageReceived);
        }

        [LuisIntent("Questao_Intent")]
        public async Task Questao(IDialogContext context, LuisResult result)
        {
            // Obtém o nome da cidade
            var questao = result.Entities.FirstOrDefault()?.Entity;

            // Mensagem que o bot vai devolver ao usuário
            //string message = "";
            if (questao.ToUpper().Equals("SALÃO"))
            {
                //await context.Forward(new FacebookLocationDialog(), ResumeAfter, context, CancellationToken.None);

                var reply = context.MakeMessage();
                reply.Attachments.Add(new HeroCard
                {
                    Title = "Abrir a sua localização no bing maps!",
                    Buttons = new List<CardAction> {
                            new CardAction
                            {
                                Title = "Sua localização",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/maps/?v=2&cp=-23.5977319~-23.5977319&lvl=16&dir=0&sty=c&sp=point.-23.5977319_-46.6821862_You%20are%20here&ignoreoptin=1"
                            }
                        }

                }.ToAttachment());

                await context.PostAsync(reply);
            }
            //else
            //{
                context.Wait(this.MessageReceived);
            //}

            // Avisa ao contexto do bot para ele mandar a mensagem para o usuário via POST
            //await context.PostAsync(message);

            // Avisa ao contexto do bot para aguardar uma mensagem de resposta do usuário 
            //context.Wait(this.MessageReceived);
        }

        public async Task ResumeAfter(IDialogContext context, IAwaitable<Place> result)
        {
            var place = await result;

            //if (place != default(Place))
            //{
                //var geo = (place.Geo as JObject)?.ToObject<GeoCoordinates>();
                //if (geo != null)
                //{
                    var reply = context.MakeMessage();
                    reply.Attachments.Add(new HeroCard
                    {
                        Title = "Abrir a sua localização no bing maps!",
                        Buttons = new List<CardAction> {
                            new CardAction
                            {
                                Title = "Sua localização",
                                Type = ActionTypes.OpenUrl,
                                Value = $"https://www.bing.com/maps/?v=2&cp=-23.5977319~-23.5977319&lvl=16&dir=0&sty=c&sp=point.-23.5977319_-46.6821862_You%20are%20here&ignoreoptin=1"
                            }
                        }

                    }.ToAttachment());

                    await context.PostAsync(reply);
                //}
                //else
                //{
                //    await context.PostAsync("No GeoCoordinates!");
                //}
            //}
            //else
            //{
            //    await context.PostAsync("No location extracted!");
            //}

            context.Wait(MessageReceived);
        }


        private DateTime RetornarData()
        {
            Random rnd = new Random();
            DateTime GenererateRandomDate()
            {
                int day = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                int Day = rnd.Next(DateTime.Now.Day, day);

                DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, Day);
                return dt;
            }

            return GenererateRandomDate();
        }
    }

    [Serializable]
    public class FacebookLocationDialog : IDialog<Place>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var msg = await argument;
            if (msg.ChannelId == "facebook")
            {
                var reply = context.MakeMessage();
                reply.ChannelData = new FacebookMessage
                (
                    text: "Please share your location with me.",
                    quickReplies: new List<FacebookQuickReply>
                    {
                        // If content_type is location, title and payload are not used
                        // see https://developers.facebook.com/docs/messenger-platform/send-api-reference/quick-replies#fields
                        // for more information.
                        new FacebookQuickReply(
                            contentType: FacebookQuickReply.ContentTypes.Location,
                            title: default(string),
                            payload: default(string)
                        )
                    }
                );
                await context.PostAsync(reply);
                context.Wait(LocationReceivedAsync);
            }
            else
            {
                context.Done(default(Place));
            }
        }

        public virtual async Task LocationReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var msg = await argument;
            var location = msg.Entities?.Where(t => t.Type == "Place").Select(t => t.GetAs<Place>()).FirstOrDefault();
            context.Done(location);
        }
    }
}