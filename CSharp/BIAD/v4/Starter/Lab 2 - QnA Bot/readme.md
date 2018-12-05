# Lab 2
## QnA Bot
Now to make things more interesting. Using [QnA Maker](https://www.qnamaker.ai/) we will prototype a bot that can answer some frequently asked questions. QnA Maker is an extremely powerful API that greatly simplifies FAQ or knowledge-base interactions.

Observe the Nuget packages included in the project and note which stem from the Microsoft.Bot.Builder namespace. These are essential to building a bot. If you are coming from V3, this project will also help you appreciate that the V4 library allows your bot projects to be far more modular.

### Prerequisites
Before beginning, be sure to have created a QnA Maker knowledge base. Which in turn requires a QnA Maker Service created on your Azure subscription. 

In the project, you will see that a QnA Service has been added to BotConfiguration.bot.

**Be sure to update the KbId, Key, and Endpoint values to match your knowledge base!**

### Steps:
1. We can pass bot-related dependencies to the bot and dialogs via DI. Start with the bot configuration
2. Configure the QnA services
3. Add a callback for "OnTurnError" that logs any bot or middleware errors to the console
4. Allow the bot to query QnA Maker on each turn

**Congratulations!** You have just integrated QnA Maker with your Bot application!