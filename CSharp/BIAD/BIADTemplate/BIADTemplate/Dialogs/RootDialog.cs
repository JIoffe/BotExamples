using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BIADTemplate.Dialogs
{
    //All IDialog implementations and all their member variables must be marked as serializable.
    //This is because BotBuilder V3 uses binary serialization to maintain the state of the conversation
    //inbetween API calls. You can omit member variables with the [NonSerialized] attribute
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            //A root dialog should probably only start once per conversation, so this can be a welcome message
            //for the first time a user messages the bot. Note: it is also possible to engage the user
            //when they "join" a conversation as well.
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            //As a root, the job of this dialog will be to route the conversation
            //to other child dialogs. This is one pattern that can work well with Bot Builder V3
            var activity = await result as IMessageActivity;
            context.Wait(MessageReceivedAsync);
        }

        /// <summary>
        /// A callback for when a child flow is complete
        /// </summary>
        private Task ResumeAfter_ChildFlow(IDialogContext context, IAwaitable<string> result)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
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
        private Task ResumeAfter_Luis(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }
    }
}