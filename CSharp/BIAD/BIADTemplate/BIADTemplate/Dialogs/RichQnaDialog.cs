using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using BIADTemplate.Model;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace BIADTemplate.Dialogs
{
    /// <summary>
    /// Qna-Powered dialog that is built to respond in RICH TEXT format
    /// </summary>
    [Serializable]
    public class RichQnaDialog : QnAMakerDialog
    {
        private static readonly double SCORE_TRESHOLD = 0.8;

        public RichQnaDialog()
            : base(new QnAMakerService(new QnAMakerAttribute(ConfigurationManager.AppSettings["QnaSubscriptionKey"], ConfigurationManager.AppSettings["QnaKnowledgebaseId"])))

        {

        }


        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {  
            var msg = context.MakeMessage();
            var answer = result.Answers.FirstOrDefault(a => a.Score >= SCORE_TRESHOLD);

            if (answer == null)
            {
                msg.Attachments.Add(BuildClarificationResponseCard(result));
            }
            else
            {
                msg.Attachments.Add(BuildResponseCard(answer));
            }

            await context.PostAsync(msg);

            //context.Done(string.Empty);
        }

        private Attachment BuildClarificationResponseCard(QnAMakerResults result)
        {
            return new HeroCard
            {
                Text = "Did you mean one of these?",
                Buttons = result.Answers.Select(a => new CardAction(ActionTypes.PostBack, title: a.Questions.First(), value: a.Questions.First())).ToList()
            }
            .ToAttachment();
        }

        private Attachment BuildResponseCard(QnAMakerResult answer)
        {
            var json = answer.Answer;
            var response = JsonConvert.DeserializeObject<RichQnaResponse>(json);

            return new HeroCard
            {
                Text = response.Title,
                Subtitle = response.Text,
                Images = new CardImage[]
                {
                    new CardImage
                    {
                        Url = response.Image
                    }
                }
            }
            .ToAttachment();
        }
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            // TODO: Put logic for handling user message here

            context.Wait(MessageReceivedAsync);
        }
    }
}