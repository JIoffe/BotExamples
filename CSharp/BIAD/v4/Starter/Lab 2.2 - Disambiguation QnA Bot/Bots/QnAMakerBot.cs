using JIoffe.BIAD.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JIoffe.BIAD.Bots
{
    public class QnAMakerBot : IBot
    {
        private const float CONFIDENCE_THRESHOLD = 0.55f;

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

            //Invoke GetAnswersAsync from the QnAMaker instance and respond to the user
            // - Send a notice if there was no response from the service
            // - Send the top answer if there was
            var response = await _qnaMaker.GetAnswersAsync(turnContext);
            if(response == null || response.Length == 0)
            {
                await turnContext.SendActivityAsync(Responses.QnA.NoAnswer, cancellationToken: cancellationToken);
            }
            else
            {
                //Step 1) Get the top result that meets or exceeds the confidence threshold
                var topResult = ???

                if(topResult == null)
                {
                    //Here we will present a card with buttons to provide potential option
                    //Step 2) Create labels from the first question of each result in the response

                    //TODO - Map each QueryResult in the response to a string. Pick the first question of each QueryResult

                    //TODO - Map each label to a button. The actionType can be either ActionTypes.ImBack or ActionTypes.PostBack.

                    //TODO - Create a hero card that includes these buttons

                    //TODO - Use MessageFactory.Attachment to create and send an activity to the user
                }
                else
                {
                    //Step 6) - Deserialize from the answer of the topresult
                    var answer = topResult.Answer;
                    var richQnaResult = JsonConvert.DeserializeObject<RichQnAResult>(answer);

                    var heroCard = new HeroCard()
                    {
                        Title = richQnaResult.Title,
                        Text = richQnaResult.Body,
                        Buttons = new[]
                        {
                            new CardAction(ActionTypes.OpenUrl, title: "Learn More", value: richQnaResult.Url)
                        },
                        Images = new[]
                        {
                            new CardImage(richQnaResult.Img)
                        }
                    };

                    var activity = MessageFactory.Attachment(heroCard.ToAttachment());
                    await turnContext.SendActivityAsync(activity, cancellationToken: cancellationToken);
                }
            }

        }
    }
}
