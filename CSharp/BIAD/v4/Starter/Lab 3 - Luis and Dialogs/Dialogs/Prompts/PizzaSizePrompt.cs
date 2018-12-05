using JIoffe.PizzaBot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PizzaBot.Dialogs.Prompts
{
    //Out of the box, Microsoft.Bot.Builder.Dialogs provides a number of useful prompts including:
    // -Freeform text
    // -Yes/No Confirmation
    // -Multiple Choice
    // -Date/Time
    // etc.

    //However, sometimes you need your own prompt. This is an example of a prompt that specifically
    //asks the user for a pizza size. This allows it to be decoupled from a particular dialog
    public class PizzaSizePrompt : Prompt<PizzaSize>
    {
        public PizzaSizePrompt(string dialogId, PromptValidator<PizzaSize> validator = null)
            : base(dialogId, validator)
        {

        }

        protected override async Task OnPromptAsync(ITurnContext turnContext, IDictionary<string, object> state, PromptOptions options, bool isRetry, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Add "suggested actions" which will appear as buttons specific to this point in the conversation
            var prompt = options?.Prompt ?? MessageFactory.Text(Responses.Orders.SizePrompt);

            //isRetry is true if this is a a retry attempt
            if (isRetry && options?.RetryPrompt != null)
            {
                prompt = options.RetryPrompt;
            }


            //Suggested Actions appear as buttons in most channels. These are specific to a single point in
            //a conversation and will NOT persist after clicked. Good for answering immediate questions
            prompt.SuggestedActions = new SuggestedActions()
            {
                Actions = new[]
                {
                    new CardAction() { Title = "Small", Type = ActionTypes.ImBack, Value = "Small" },
                    new CardAction() { Title = "Medium", Type = ActionTypes.ImBack, Value = "Medium" },
                    new CardAction() { Title = "Large", Type = ActionTypes.ImBack, Value = "Large" },
                }
            };

            await turnContext.SendActivityAsync(prompt);
        }

        protected override Task<PromptRecognizerResult<PizzaSize>> OnRecognizeAsync(ITurnContext turnContext, IDictionary<string, object> state, PromptOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var input = turnContext.Activity.Value as string ?? turnContext.Activity.Text;

            PizzaSize size = 0;
            var success = Enum.TryParse(input, true, out size);
     
            //If "succeeded" is false, control will not go to the next step and will instead 
            //return to OnPromptAsync
            var result = new PromptRecognizerResult<PizzaSize>()
            {
                Succeeded = success,
                Value = size
            };

            return Task.FromResult(result);
        }
    }
}
