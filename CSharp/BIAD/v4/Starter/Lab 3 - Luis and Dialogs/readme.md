# Lab 3
## Pizza Ordering Bot
Time to build something more practical. This bot will leverage Luis to predict the intent of our users and drive a conversation towards ordering a pizza.

This example will demonstrate the basics of using dialogs and dialog state with Azure Bot Services. Think of the dialog state as a stack, and each point in our conversation can either push or pop from this stack. Additionally, anytime we move forward or backward, we are able to consider the previous results. This lets us quickly create branches of conversations, or preset steps that flow like a waterfall.

Additionally, we will learn how to maintain a persistent state per user per conversation. In this example, the state is the pizza order; this includes the address, size, and any toppings. But truly, it can be anything. Think of it as something akin to the ViewBag in a razor page: you can leave information here that is convenient to have tied to a conversation.

Observe the Nuget packages included in the project and note which stem from the Microsoft.Bot.Builder namespace. These are essential to building a bot. If you are coming from V3, this project will also help you appreciate that the V4 library allows your bot projects to be far more modular.

### Prerequisites
Before beginning, you should become acquainted with [Luis.ai](https://www.luis.ai/). To get started directly with the intents in this sample code, a sample model is available in the root of the v4 folder, under "AI Models".

In the project, you will see that a QnA Service has been added to BotConfiguration.bot.

Additionally, update the appsettings.json file to reflect your unique Luis app settings.

### Steps:
1.	Create a new DialogSet in the constructor of PizzaOrderBot.cs. Use the PizzaOrderAccessors passed via dependency injection.
2.	Use MainDialogFactory and OrderDialogFactory to add dialogs to the DialogSet
3.	Add prompts to gather the user’s address, confirm the user’s address, and to confirm the order
4.	Add additional prompts for sizes and toppings
5.	Changes need to be made on OnTurnAsync to utilize dialogs more effectively. We can also begin exploring other types of activities. Start by greeting a user when the activity type is “conversationUpdate” and the number of members added is greater than 0. Be mindful that the bot itself also joins the conversation and sends itself a conversationUpdate event.
6.	Recreate the DialogContext from the DialogSet on each turn and continue the active dialog
7.	If we did not respond for this turn, begin the main menu dialog
8.	Extremely important! Update the conversation state at the end of each turn
9.	Pass the ITurnContext into the LuisRecognizer to get the top scoring intent for this turn.
10.	In MainMenuDialogFactory, if the score is below a confidence threshold, show confusion response and end dialog.
11.	Create a switch statement against the intent. For greetings, send a welcome message. For help, send a help message. Default to confusion.
12.	In PizzaOrderDialogFactory, create a new waterfall dialog covering the full range of steps to order a pizza: GetAddress, ConfirmAddress, BeginOrder, GetSize, GetToppings, ConfirmOrder and finally CompleteOrder
13.	In GetAddressAsync, pull the PizzaOrderState from the injected accessors
14.	If we already have a preferred address, then skip to the next step!
15.	Otherwise, prompt the user to enter an address. This will be passed to the next step of the waterfall.
16.	Retrieve the address from the current step context result.
17.	Prompt the user to confirm that this is the correct address to use.
18.	If this is not the correct address, we will start over! Get the order state from the accessors again, and this time set the address to empty. Save all changes and use ReplaceDialog to restart this dialog.
19.	Check if the current order already has a size (it is not set to 0). If so, move to the next step with the size. Otherwise, prompt for the size.
20.	Check if the current order already has a toppings (it is not set to 0). If so, move to the next step with topping selection. Otherwise, prompt for the toppings.
21.	Observe the same ideas in the next step. In SendReceiptAsync, create and send a new ReceiptCard detailing the customer’s order
22.	In CompleteOrderAsync, cast the result to (bool) to see if the user clicked yes or no. Respond accordingly.
23.	End the dialog. This will bring us back to the previous point on the stack.
24.	In Startup.cs, it is now time to add the new service dependencies. Start with adding a singleton for PizzaOrderAccessors on line 101. First, retrieve the bot options from the IServiceCollection. You can use the type IOptions<BotFrameowkrOptions>
25.	Retrieve the ConversationState from the options.
26.	Now return a new instance of PizzaOrderAccessors based on this conversation state. We also need to register new properties for each state we’re tracking – the “PizzaOrderState” and the “DialogState”
27.	Add a singleton of type LuisRecognizer to the services container. This involves creating a new instance of LuisApplication and LuisPredictionOptions.
28.	Step 28) The last setup step for this lab. Add singletons for "MainMenuDialogFactory" and "PizzaOrderDialogFactory" to the services container.


**Congratulations!**
Okay, that was a lot to go through! At this point though, you have a very functional bot. As an added challenge, try to combine QnA and Luis into a single application. 

*Hint: Try to fallback to QnA if Luis fails to make a confident prediction.*