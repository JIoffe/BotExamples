using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using System.Threading;
using System.Threading.Tasks;

namespace JIoffe.BIAD.Bots
{
    public class QnAMakerBot : IBot
    {
        //Step 4) Allow the bot to query QnA Maker on each turn

        //TODO - Add a private member of type QnAMaker and initialize it via Dependency Injection

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Ignore activities that are not messages
            if (turnContext.Activity.Type != "message")
                return;

            //TODO - Invoke GetAnswersAsync on the QnAMaker instance; save the response to a variable

            //TODO - If the response is null or empty send a failure notice to the user

            //TODO - Otherwise, send the answer of the first response to the user
        }
    }
}
