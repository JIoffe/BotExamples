using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using System.Threading;
using System.Threading.Tasks;

namespace JIoffe.BIAD.Bots
{
    public class QnAMakerBot : IBot
    {
        //Step 4) Allow the bot to respond to QnA on each turn

        //Initialize a private variable of QnAMaker via Dependency Injection
        private readonly QnAMaker _qnaMaker;

        public QnAMakerBot(QnAMaker qnaMaker)
        {
            _qnaMaker = qnaMaker;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Ignore activities that are not messages
            if (turnContext.Activity.Type != "message")
                return;

            //Step X) Invoke GetAnswersAsync from the QnAMaker instance and respond to the user
            // - Send a notice if there was no response from the service
            // - Send the top answer if there was
            var response = await _qnaMaker.GetAnswersAsync(turnContext);
            if(response == null || response.Length == 0)
            {
                await turnContext.SendActivityAsync(Responses.QnA.NoAnswer, cancellationToken: cancellationToken);
            }
            else
            {
                var answer = response[0].Answer;
                await turnContext.SendActivityAsync(answer, cancellationToken: cancellationToken);
            }

        }
    }
}
