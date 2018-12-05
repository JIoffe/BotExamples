using JIoffe.PizzaBot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
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
    //asks the user for pizza toppings. This allows it to be decoupled from a particular dialog

    //Also, due to the platform constraints, there is no out-of-the-box solution for multiple selections.
    //This is one potential solution - repeating the prompt with reduced options each time

    public class PizzaToppingPrompt : Prompt<ISet<PizzaTopping>>
    {
        private const string SELECTION_DONE = "Done with toppings";
        private const string SELECTIONS_KEY = "SELECTIONS";

        public PizzaToppingPrompt(string dialogId, PromptValidator<ISet<PizzaTopping>> validator = null)
            : base(dialogId, validator)
        {

        }

        protected override async Task OnPromptAsync(ITurnContext turnContext, IDictionary<string, object> state, PromptOptions options, bool isRetry, CancellationToken cancellationToken = default(CancellationToken))
        {
            IMessageActivity activity;
            
            if (GetSelectionsFromState(state).Count > 0)
            {
                activity = GetPizzaToppingCarousel(state, Responses.Orders.AdditionalToppingsPrompt);
                activity.SuggestedActions = new SuggestedActions()
                {
                    Actions = new[]
                    {
                        new CardAction() { Title = Responses.Orders.ToppingsDone, Type = ActionTypes.PostBack, Value = SELECTION_DONE }
                    }
                };
            }
            else
            {
                var caption = options?.Prompt?.Text ?? Responses.Orders.ToppingsPrompt;
                activity = GetPizzaToppingCarousel(state, caption);
            }

            await turnContext.SendActivityAsync(activity);
        }

        protected override async Task<PromptRecognizerResult<ISet<PizzaTopping>>> OnRecognizeAsync(ITurnContext turnContext, IDictionary<string, object> state, PromptOptions options, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Valid inputs are either DONE or a pizza topping
            var input = turnContext.Activity.Value as string ?? turnContext.Activity.Text;
            var completedSelection = input.Equals(SELECTION_DONE, StringComparison.InvariantCultureIgnoreCase);

            PizzaTopping topping = 0;
            if (Enum.TryParse(input, true, out topping))
            {
                AddToppingToState(state, topping);
                completedSelection = HasSelectedAllOptions(state);
            }
            else if (!completedSelection)
            {
                //If it's not a topping and we're not DONE, then this is an invalid input
                //The reprompt is automatic if we did NOT respond for this turn. 
                await turnContext.SendActivityAsync(string.Format(Responses.Orders.ToppingsReprompt, input));
                await OnPromptAsync(turnContext, state, options, true, cancellationToken);
            }

            return new PromptRecognizerResult<ISet<PizzaTopping>>()
            {
                Succeeded = completedSelection,
                Value = GetSelectionsFromState(state)
        };
        }

        private IMessageActivity GetPizzaToppingCarousel(IDictionary<string, object> state, string caption)
        {
            //Turn all the pizza topping enum values into hero cards
            //Remove any the user has already selected
            var attachments = ((PizzaTopping[])Enum.GetValues(typeof(PizzaTopping)))
                .Except(GetSelectionsFromState(state))
                .Select(topping =>
                {
                    var label = topping.ToString();
                    return new HeroCard()
                    {
                        Title = label,
                        Tap = new CardAction() { Type = ActionTypes.ImBack, Value = label },
                        Images = new[]
                        {
                            new CardImage() { Url = "https://via.placeholder.com/150" }
                        }
                    }.ToAttachment();
                });

            //Now combine them into a carousel
            var activity = MessageFactory.Carousel(attachments, text: caption);
            return activity;
        }

        private ISet<PizzaTopping> GetSelectionsFromState(IDictionary<string, object> state)
        {
            if (!state.ContainsKey(SELECTIONS_KEY))
            {
                state[SELECTIONS_KEY] = new HashSet<PizzaTopping>();
            }

            return state[SELECTIONS_KEY] as ISet<PizzaTopping>;
        }
        private void AddToppingToState(IDictionary<string, object> state, PizzaTopping topping)
        {
            var selections = GetSelectionsFromState(state);
            selections.Add(topping);
        }

        private bool HasSelectedAllOptions(IDictionary<string, object> state)
            => GetSelectionsFromState(state).Count == Enum.GetValues(typeof(PizzaTopping)).Length;
    }
}
