using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace BIADTemplate.Dialogs
{
    //All IDialog implementations and all their member variables must be marked as serializable.
    //This is because BotBuilder V3 uses binary serialization to maintain the state of the conversation
    //inbetween API calls. You can omit member variables with the [NonSerialized] attribute
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        //StartAsync is the only interface method that all dialogs must implement
        //This is invoked by the bot framework when a dialog is added to the conversation stack.
        //Note: we might not even have a message from the user yet!
        public Task StartAsync(IDialogContext context)
        {
            //This conversation is now at the top of the dialog stack.
            //Wait for input from the user before continuing.
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            //The bot builder SDK V3 relies heavily on callbacks. In this case,
            //"result" in this case holds what we received from the user

            //And in this case, we will just parrot it back to the user.
            var activity = await result as IMessageActivity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync($"You sent {activity.Text} which was {length} characters long.");

            //This is essentially a loop. The next time the user sends a message to the same context,
            //we will process using this same callback.
            context.Wait(MessageReceivedAsync);
        }
    }
}