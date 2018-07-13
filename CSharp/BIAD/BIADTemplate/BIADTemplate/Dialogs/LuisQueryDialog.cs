using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace BIADTemplate.Dialogs
{
    /// <summary>
    /// Simplistic example of how to use LUIS to route a conversation
    /// </summary>
    [Serializable]
    public class LuisQueryDialog: LuisDialog<object>
    {
        //In a production application, there is nothing stopping you from 
        //querying LUIS at any point in your processing.

        private static readonly double SCORE_THRESHOLD = 0.8D;

        public LuisQueryDialog()
            : base(GetLuisService())
        {

        }


        //The LuisIntent attribute associates a callback with a LUIS intent
        //The callback is passed the original LUIS result, as well as the user's input
        //By passing multiple attributes, we can relate a callback to multiple intents.
        //By passing blank and "None", we essentially have a catch all for all intents
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, IAwaitable<IMessageActivity> item, LuisResult result)
        {
            var intent = result.TopScoringIntent.Intent;
            await context.PostAsync($"Recognized the intent: {intent}");
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