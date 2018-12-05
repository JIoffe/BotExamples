using Microsoft.Bot.Builder;
using System.Threading;
using System.Threading.Tasks;

namespace PizzaBot.Bots
{
    /// <summary>
    /// Bot that echos back a user's responses on each turn
    /// </summary>
    public class EchoBot : IBot
    {
        //The IBot interface is provided by the Microsoft.Bot.Builder package.
        //It lets us define behavior that happens on each "turn" of a bot.
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
