using JIoffe.PizzaBot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using PizzaBot.Accessors;
using PizzaBot.States;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PizzaBot.Dialogs
{
    //The methods in this class might seem a little repetetive; this is
    //to really make clear the basic flows available in Bot.Builder.Dialogs

    /// <summary>
    /// Handles all the steps related to taking a pizza order as WaterfallSteps
    /// </summary>
    public class PizzaOrderDialogFactory : IDialogFactory
    {
        private readonly PizzaOrderAccessors _accessors;
        private readonly LuisRecognizer _luisRecognizer;

        public PizzaOrderDialogFactory(PizzaOrderAccessors accessors, LuisRecognizer luisRecognizer)
        {
            _accessors = accessors;
            _luisRecognizer = luisRecognizer;
        }

        public Dialog Build()
        {
            //Step 12) Create a new waterfall dialog covering the full range of 
            //steps to order a pizza: GetAddress, ConfirmAddress, BeginOrder, GetSize, GetToppings, 
            //ConfirmOrder and finally CompleteOrder
            var steps = new WaterfallStep[]
            {
                GetAddressAsync,
                ConfirmAddressAsync,
                BeginOrderAsync,
                GetSizeAsync,
                GetToppingsAsync,
                ConfirmOrderAsync,
                CompleteOrderAsync
            };

            var dialog = new WaterfallDialog(DialogNames.PIZZA_ORDER, steps);
            return dialog;
        }

        private async Task<DialogTurnResult> GetAddressAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //Step 13) Pull the PizzaOrderState from our accessors. 
            var turnContext = stepContext.Context;
            var state = await _accessors.PizzaOrderState.GetAsync(turnContext, () => new States.PizzaOrderState());

            //Step 14) If we already have a preferred address, then skip to the next step!
            if (!string.IsNullOrWhiteSpace(state.PreferredAddress))
            {
                return await stepContext.NextAsync(state.PreferredAddress, cancellationToken);
            }

            //Step 15) Otherwise, prompt the user to enter an address. This will be passed to the next step of the waterfall.
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(Responses.Orders.AddressPrompt)
            };

            return await stepContext.PromptAsync(DialogNames.ADDRESS_PROMPT, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmAddressAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //Step 16) Retrieve the address from the current step context result
            var address = stepContext.Result as string;
            address = address.Trim();

            var turnContext = stepContext.Context;
            var state = await _accessors.PizzaOrderState.GetAsync(turnContext, () => new States.PizzaOrderState());
            state.PreferredAddress = address;
            await _accessors.ConversationState.SaveChangesAsync(turnContext);

            //Step 17) Prompt the user to confirm that this is the correct address to use
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(string.Format(Responses.Orders.AddressConfirm, address))
            };

            return await stepContext.PromptAsync(DialogNames.ADDRESS_CONFIRM, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> BeginOrderAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //The choice prompt will provide us with a boolean.
            var correctAddress = (bool)stepContext.Result;

            var turnContext = stepContext.Context;

            //Step 18) If this is not the correct address, we will start over! 
            // Get the order state from the accessors again, and this time set the address to empty. 
            // Save all changes and use ReplaceDialog to restart this dialog.
            if (!correctAddress)
            {
                //Clear the address
                var state = await _accessors.PizzaOrderState.GetAsync(turnContext, () => new States.PizzaOrderState());
                state.PreferredAddress = string.Empty;
                await _accessors.ConversationState.SaveChangesAsync(turnContext);

                //ReplaceDialogAsync can be used to restart a dialog
                return await stepContext.ReplaceDialogAsync(DialogNames.PIZZA_ORDER, null, cancellationToken);
            }

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> GetSizeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //Step 19) Check if the current order already has a size (it is not set to 0). 
            //If so, move to the next step with the size. Otherwise, prompt for the size.
            var turnContext = stepContext.Context;
            var state = await _accessors.PizzaOrderState.GetAsync(turnContext, () => new States.PizzaOrderState());

            if (state.CurrentOrder.Size != 0)
            {
                return await stepContext.NextAsync(state.CurrentOrder.Size);
            }

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(Responses.Orders.SizePrompt)
            };
            return await stepContext.PromptAsync(DialogNames.SIZE_PROMPT, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> GetToppingsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //Step 20) Check if the current order already has a toppings (it is not set to 0).
            // If so, move to the next step with topping selection. Otherwise, prompt for the toppings.
            var turnContext = stepContext.Context;

            var state = await _accessors.PizzaOrderState.GetAsync(turnContext, () => new States.PizzaOrderState());
            state.CurrentOrder.Size = (PizzaSize)stepContext.Result;
            await _accessors.ConversationState.SaveChangesAsync(turnContext);

            if(state.CurrentOrder.Toppings?.Count > 0)
            {
                return await stepContext.NextAsync(state.CurrentOrder.Toppings);
            }

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(Responses.Orders.ToppingsPrompt)
            };
            return await stepContext.PromptAsync(DialogNames.TOPPINGS_PROMPT, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmOrderAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //This is similar to the earlier entries, so it will be provided for you.
            var turnContext = stepContext.Context;

            var state = await _accessors.PizzaOrderState.GetAsync(turnContext, () => new States.PizzaOrderState());
            state.CurrentOrder.Toppings = (ISet<PizzaTopping>)stepContext.Result;
            await _accessors.ConversationState.SaveChangesAsync(turnContext);

            //Bot Framework gives us a number of pre-built cards that can be interpreted
            //by several platforms. In this method, let us implement a basic ReceiptCard.
            await SendReceiptAsync(turnContext, state);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(Responses.Orders.ConfirmOrder)
            };
            return await stepContext.PromptAsync(DialogNames.CONFIRM_ORDER, promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> CompleteOrderAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var turnContext = stepContext.Context;
            var state = await _accessors.PizzaOrderState.GetAsync(turnContext, () => new States.PizzaOrderState());

            //Step 22 - Cast the result to (bool) to see if the user clicked yes or no. Respond accordingly.
            var orderConfirmed = (bool)stepContext.Result;
            if (orderConfirmed)
            {
                var response = string.Format(Responses.Orders.CompletedOrder, state.CurrentOrder.Toppings.First());
                await turnContext.SendActivityAsync(response);
            }
            else
            {
                await turnContext.SendActivityAsync(Responses.Orders.CancelledOrder);
            }

            //Step 23 - End the dialog. This will bring us back to the previous point on the stack.
            return await stepContext.EndDialogAsync();
        }

        private async Task SendReceiptAsync(ITurnContext context, PizzaOrderState state)
        {
            var items = GetReceiptItems(state.CurrentOrder);
            var total = items.Sum(item => double.Parse(item.Price));

            //Step 21 - Create and send a new ReceiptCard detailing the customer's order.

            var msg = context.Activity.CreateReply();
            msg.Attachments = new[]
            {
                new ReceiptCard()
                {
                    Title = $"Pizza Order - {state.PreferredAddress}",              
                    Facts = new[]
                    {
                        new Fact(){Key = "Address", Value = state.PreferredAddress}
                    },
                    Total = $"${total.ToString("N2")}",
                    Items = items
                }.ToAttachment()
            };

            await context.SendActivityAsync(msg);
        }

        private IList<ReceiptItem> GetReceiptItems(PizzaOrder order)
        {
            //Extremely simple for the sake of this example
            var sizePrices = new[] { 10D, 15D, 20D };
            var toppingPrices = new[] { 2D, 1.5D, 2.5D, 3D, 1D, 1.5D, 2D, 3D };

            var items = new List<ReceiptItem>();

            items.Add(new ReceiptItem(title: $"{order.Size} pie", price: sizePrices[(int)order.Size - 1].ToString("N2")));
            items.AddRange(order.Toppings.Select(t => new ReceiptItem(title: $"+{t}", price: toppingPrices[(int)t- 1].ToString("N2"))));

            return items;
        }
    }
}
