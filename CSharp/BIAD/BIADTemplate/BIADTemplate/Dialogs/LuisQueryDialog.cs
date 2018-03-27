using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace BIADTemplate.Dialogs
{
    /// <summary>
    /// Extremely simplistic example of how to use LUIS to route a conversation
    /// </summary>
    [Serializable]
    public class LuisQueryDialog: LuisDialog<object>
    {
        private static readonly double SCORE_THRESHOLD = 0.8D;

        public LuisQueryDialog()
            : base(GetLuisService())
        {

        }

        [LuisIntent("Greeting")]
        public async Task GreetingIntent(IDialogContext context, IAwaitable<IMessageActivity> item, LuisResult result)
        {
            await context.PostAsync("Yeah! Hello! What can I do for you?");
            context.Done(string.Empty);
        }

        [LuisIntent("Help")]
        public async Task HelpIntent(IDialogContext context, IAwaitable<IMessageActivity> item, LuisResult result)
        {
            await context.PostAsync("I'm here to help you order pizza and learn about building chatbots with Microsoft's BotBuilder Framework!\r\nYou can also ask me the time.");
            context.Done(string.Empty);
        }

        [LuisIntent("Time")]
        public async Task TimeIntent(IDialogContext context, IAwaitable<IMessageActivity> item, LuisResult result)
        {
            var est = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            await context.PostAsync($"Ding dong! It is now {est.ToShortTimeString()}");
            context.Done(string.Empty);
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, IAwaitable<IMessageActivity> item, LuisResult result)
        {
            //Pass the unprocessed item back into root
            context.Done(await item);
        }

        private static ILuisService GetLuisService()
        {
            var appId = ConfigurationManager.AppSettings["LuisAppId"];
            var apiKey = ConfigurationManager.AppSettings["LuisAPIKey"];

            var attribute = new LuisModelAttribute(appId, apiKey, threshold: SCORE_THRESHOLD);

            return new LuisService(attribute);
        }
    }
}