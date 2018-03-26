using System;
using System.Threading.Tasks;
using BIADTemplate.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BIADTemplate.Dialogs.Prompts
{
    [Serializable]
    public class PieSizePrompt : IDialog<PizzaOrder.PieSize>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var msg = context.MakeMessage();
            msg.Attachments.Add(BuildPieSizeCard());

            await context.PostAsync(msg);
            context.Wait(SizeSelected);
        }

        private async Task SizeSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
            var response = activity.Value as string ?? activity.Text;

            var size = Enum.Parse(typeof(PizzaOrder.PieSize), response);
            context.Done(size);
        }

        private Attachment BuildPieSizeCard()
        {
            return new HeroCard
            {
                Title = "Which pie size would you like?",
                Buttons = new CardAction[]
                {
                    new CardAction
                    {
                        Type = ActionTypes.PostBack,
                        Title = "Small",
                        Value = "SMALL"
                    },
                    new CardAction
                    {
                        Type = ActionTypes.PostBack,
                        Title = "Medium",
                        Value = "MEDIUM"
                    },
                    new CardAction
                    {
                        Type = ActionTypes.PostBack,
                        Title = "Large",
                        Value = "LARGE"
                    }
                }
            }.ToAttachment();
        }
    }
}