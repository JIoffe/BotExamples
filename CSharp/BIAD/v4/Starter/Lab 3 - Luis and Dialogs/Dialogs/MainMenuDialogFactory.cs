using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JIoffe.PizzaBot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using PizzaBot.Accessors;
using PizzaBot.Responses;

namespace PizzaBot.Dialogs
{
    public class MainMenuDialogFactory : IDialogFactory
    {
        private readonly PizzaOrderAccessors _accessors;
        private readonly LuisRecognizer _luisRecognizer;

        /// <summary>
        /// Predictions with lower scores than this threshold should be ignored
        /// </summary>
        private const double CONFIDENCE_THRESHOLD = 0.6D;

        public MainMenuDialogFactory(PizzaOrderAccessors accessors, LuisRecognizer luisRecognizer)
        {
            _accessors = accessors;
            _luisRecognizer = luisRecognizer;
        }

        public Dialog Build()
        {
            //The WaterfallDialog is a type of dialog that can
            //have one or many steps that flow into eachother linearly.
            //The result of one Waterfall step will carry to the start 
            //of the next one.

            //At any point, you are able to push or pop from the dialog stack.
            //Observe how the steps are applied here so that you can create
            //the steps necessary for placing an order.
            var steps = new WaterfallStep[]
            {
                MainMenuAsync
            };

            var dialog = new WaterfallDialog(DialogNames.MAIN_MENU, steps);
            return dialog;
        }

        private async Task<DialogTurnResult> MainMenuAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var turnContext = stepContext.Context;

            //Step 9) Pass the ITurnContext into the LuisRecognizer to get the top scoring intent for this turn.
            var recognizerResult = await _luisRecognizer.RecognizeAsync(turnContext, cancellationToken);
            (string intent, double score) = recognizerResult.GetTopScoringIntent();

            //Step 10) If the score is below a confidence threshold, show confusion response and end dialog
            if(score < CONFIDENCE_THRESHOLD)
            {
                await turnContext.SendActivityAsync(MainMenu.Confusion, cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync();
            }


            //Step 11) Create a switch statement against the intent. For "Greeting", send a welcome message. 
            //For "Help", send a help message. Default to confusion. These should match whatever you named
            //your intent(s) in Luis.ai.

            //We will return to this method in the future to support the "OrderPizza" intent for ordering pizza.
            //Use the ApplyEntitiesToOrderState helper method to flesh out an order state using Luis.ai entities.
            switch (intent)
            {
                case "Greeting":
                    await turnContext.SendActivityAsync(MainMenu.WelcomeReturningUser, cancellationToken: cancellationToken);
                    break;
                case "Help":
                    await turnContext.SendActivityAsync(MainMenu.Help, cancellationToken: cancellationToken);
                    break;
                case "OrderPizza":
                    //Clear the previous state and apply any relevant LUIS 
                    await ApplyEntitiesToOrderState(turnContext, recognizerResult);
                    return await stepContext.BeginDialogAsync(DialogNames.PIZZA_ORDER, null, cancellationToken);
                default:
                    await turnContext.SendActivityAsync(MainMenu.Confusion, cancellationToken: cancellationToken);
                    break;
            }

            //After all is over, we will end this dialog.
            //If it is the last dialog in the stack, we will recreate it
            //in OnTurnAsync. Alternatively, we could simply replace this instance
            //of a MainMenu dialog with a new one.
            return await stepContext.EndDialogAsync();
        }

        private async Task ApplyEntitiesToOrderState(ITurnContext turnContext, RecognizerResult recognizerResult)
        {
            var state = await _accessors.PizzaOrderState.GetAsync(turnContext, () => new States.PizzaOrderState());
            state.CurrentOrder = new PizzaOrder();

            //This direct enum parse will not capture pluralizations or alternate spellings!
            //But it is a straightforward example of how to apply the entities into the state
            var sizeEntities = recognizerResult.Entities.SelectToken("Size");
            var toppingEntities = recognizerResult.Entities.SelectToken("Topping");

            if(sizeEntities != null)
            {
                var label = JsonConvert.DeserializeObject<IEnumerable<string>>(JsonConvert.SerializeObject(sizeEntities)).FirstOrDefault();
                PizzaSize size;
                if(Enum.TryParse(label, true, out size))
                {
                    state.CurrentOrder.Size = size;
                }
            }

            if(toppingEntities != null)
            {
                var labels = JsonConvert.DeserializeObject<IEnumerable<string>>(JsonConvert.SerializeObject(toppingEntities));

                state.CurrentOrder.Toppings = labels.Select(label =>
                {
                    PizzaTopping topping = 0;
                    Enum.TryParse(label, true, out topping);
                    return topping;
                })
                .Where(t => t != 0)
                .ToHashSet();
            }

            await _accessors.ConversationState.SaveChangesAsync(turnContext);
        }
    }
}
