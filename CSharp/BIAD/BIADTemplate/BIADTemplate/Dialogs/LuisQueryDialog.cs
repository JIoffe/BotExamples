using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace BIADTemplate.Dialogs
{
    /// <summary>
    /// Extremely simplistic example of how to use LUIS to route a conversation
    /// </summary>
    [Serializable]
    public class LuisQueryDialog: LuisDialog<string>
    {
        public static ILuisService GetLuisService()
        {
            var appId = ConfigurationManager.AppSettings["LuisAppId"];
            var apiKey = ConfigurationManager.AppSettings["LuisAPIKey"];
            var attribute = new LuisModelAttribute(appId, apiKey);

            return new LuisService(attribute);
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public Task NoneIntent(IDialogContext context, LuisResult result)
        {
            context.Done("none");
            return Task.CompletedTask;
        }
    }
}