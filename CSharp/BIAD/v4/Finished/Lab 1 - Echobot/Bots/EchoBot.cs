using Microsoft.Bot.Builder;
using Microsoft.Bot.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace BotWorkshop.Bots
{
    //The IBot interface is provided by the Microsoft.Bot.Builder package.
    //It lets us define behavior that happens on each "turn" of a bot.

    //Step 1) Have the EchoBot class implement IBot from Microsoft.Bot.Builder
    public class EchoBot : IBot
    {
        //Step 2) Implement OnTurnAsync from IBot
        // - If the incoming activity type is "Message" then repeat the user's input back
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var activity = turnContext.Activity;

            if(activity.Type == "message")
            {
                var userInput = activity.Text;
                await turnContext.SendActivityAsync($"You said: {userInput}");
            }

        }
    }
}
