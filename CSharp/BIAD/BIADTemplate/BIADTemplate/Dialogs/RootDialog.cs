using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BIADTemplate.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            context.Wait(MessageReceivedAsync);
        }

        /// <summary>
        /// A callback for when QnA is complete
        /// </summary>
        private Task ResumeAfter_Qna(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        /// <summary>
        /// A callback for when a LUIS query is complete. It will defer to QNA if LUIS does not match to a handler
        /// </summary>
        private async Task ResumeAfter_Luis(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            if(activity != null)
            {
                await context.Forward(new RichQnaDialog(), ResumeAfter_Qna, activity, CancellationToken.None);
                return;
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}