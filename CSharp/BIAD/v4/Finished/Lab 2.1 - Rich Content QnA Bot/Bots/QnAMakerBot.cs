﻿using JIoffe.BIAD.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace JIoffe.BIAD.Bots
{
    public class QnAMakerBot : IBot
    {
        //Step X) Initialize a private variable of QnAMaker via Dependency Injection
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
                var answer = response[0].Answer;

                //Step 1) Convert the answer to an instance of RichQnAResult
                var richQnaResult = JsonConvert.DeserializeObject<RichQnAResult>(answer);

                //Step 2) Create a hero card based on the content of the richQnaResult
                //  - Set the title and text to match the rich result's title and text
                //  - Add a button of type OpenUrl to navigate to the url in the result
                //  - Add the image based on the img property of the result
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

                //Step 3) Create a new activity that we can attach the hero card to
                var activity = MessageFactory.Attachment(heroCard.ToAttachment());

                //Step 4) Send the activity to the user
                await turnContext.SendActivityAsync(activity, cancellationToken: cancellationToken);
            }

        }
    }
}
