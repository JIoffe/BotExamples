# Lab 1
## Echo Bot
This is the starting point for all V4 bots running in ASP DotNetCore. We will go over the basics of configuring your web application to host a bot as well as testing the bot in the V4 Bot Channel Emulator.

Observe the Nuget packages included in the project and note which stem from the Microsoft.Bot.Builder namespace. These are essential to building a bot. If you are coming from V3, this project will also help you appreciate that the V4 library allows your bot projects to be far more modular.

### Steps:
1. Have the EchoBot class implement IBot from Microsoft.Bot.Builder
2. Implement OnTurnAsync from IBot; If the incoming activity type is "Message" then repeat the user's input back
3. Add an instance of EchoBot to the services container
4. Invoke the IApplicationBuilder extension method UseBotFramework() to automatically map endpoint handlers for bots

More details can be seen as comments in the code.


**Congratulations!** You have just built your first bot!