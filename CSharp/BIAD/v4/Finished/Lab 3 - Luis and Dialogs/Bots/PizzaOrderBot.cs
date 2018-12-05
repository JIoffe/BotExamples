using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Logging;
using PizzaBot.Accessors;
using PizzaBot.Dialogs;
using PizzaBot.Dialogs.Prompts;
using PizzaBot.Responses;
using PizzaBot.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PizzaBot.Bots
{
    //STEP 1) Implement the IBot interface from Microsoft.Bot.Builder

    /// <summary>
    /// A bot that uses LUIS, QnA Maker and branching dialogs to take pizza orders
    /// </summary>
    public class PizzaOrderBot : IBot
    {
        private readonly PizzaOrderAccessors _accessors;
        private readonly ILogger<PizzaOrderBot> _logger;
        private readonly DialogSet _dialogs;

        //This PizzaOrderBot receives the PizzaOrderAccessors and dialog factories via Dependency Injection
        //By abstracting the creation of dialogs into a factory, we create something that is reusable and testable!
        public PizzaOrderBot(PizzaOrderAccessors accessors, MainMenuDialogFactory mainDialogFactory, PizzaOrderDialogFactory orderDialogFactory, ILogger<PizzaOrderBot> logger)
        {
            _accessors = accessors;
            _logger = logger;

            //Step 1) Create a new DialogSet based on the PizzaOrderAccessors DialogState
            //A DialogSet is a grouping of dialogs that can transition between each other.
            _dialogs = new DialogSet(_accessors.DialogState);

            //Step 2) Add the Main and Order dialogs 
            _dialogs.Add(mainDialogFactory.Build());
            _dialogs.Add(orderDialogFactory.Build());

            //STEP 3) Add the following built-in prompts to the dialog set:
            // - TextPrompt for the address
            // - ConfirmPrompt to confirm the address
            // - ConfirmPrompt to confirm the order
            _dialogs.Add(new TextPrompt(DialogNames.ADDRESS_PROMPT, AddressValidator.ValidateAddressAsync));
            _dialogs.Add(new ConfirmPrompt(DialogNames.ADDRESS_CONFIRM));
            _dialogs.Add(new ConfirmPrompt(DialogNames.CONFIRM_ORDER));

            //Step 4) Add prompts for size and toppings. These can be text prompts
            // or use the provided PizzaSizePrompt and PizzaToppingPrompt dialogs.
            // Custom prompt dialogs can be created to make a consistent user experience.
            _dialogs.Add(new PizzaSizePrompt(DialogNames.SIZE_PROMPT));
            _dialogs.Add(new PizzaToppingPrompt(DialogNames.TOPPINGS_PROMPT));
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            //An "activity" is anything the user does to interact with the bot. It could
            //mean typing, joining a conversation, sending a file, recording speech, etc.
            var activity = turnContext.Activity;

            //Step 5) 
            //Greet a new user when the activity type is "conversationUpdate" and MembersAdded are greater than 0
            //Be mindful that the bot itself also joins the conversation and sends itself a conversationUpdate event.
            if(activity.Type == "conversationUpdate")
            {
                //Conversation update is an event that typically triggers
                //when users enter or leave a conversation. Good point to say hello!
                //However, both the bot and the user each individually join the conversation
                var totalNewMembers = activity.MembersAdded.Count(account => account.Id != turnContext.Activity.Recipient.Id);
                if(totalNewMembers > 0)
                {
                    await turnContext.SendActivityAsync(MainMenu.WelcomeNewUser);
                }
            }
            else if (activity.Type == "message")
            {
                //Step 6) If the activity type is message, then we have to advance the conversation. 
                // Recreate the DialogContext from the DialogSet on each turn and continue the active dialog
                //The Dialog Context has to be recreated on each turn
                var dc = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                var result = await dc.ContinueDialogAsync(cancellationToken);

                //Step 7) If we did not respond for this turn, begin the main menu dialog
                //There are a few ways to determine that we've exhausted our dialog stack.
                //In this case, we can tell if we responded for a particular turn or not.
                if (!turnContext.Responded)
                {
                    await dc.BeginDialogAsync(DialogNames.MAIN_MENU, null, cancellationToken);
                }
            }

            //Step 8) Extremely important! Update the conversation state at the end of each turn
            //Otherwise, we may encounter duplicate messages or trouble traversing the dialog stack
            await _accessors.ConversationState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
        }
    }
}
