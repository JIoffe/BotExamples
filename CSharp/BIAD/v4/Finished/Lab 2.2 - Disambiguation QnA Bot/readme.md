# Lab 2.2
## QnA Bot with disambiguation
Let us make our bot more intelligent. Let us add a way for the bot to clarify the user's question in case it does not meet a certain threshold.

We are able to do this because QnA Maker (and most cognitive services) are able to send us a full list of predictions when there may be some confusion. Using this information, we can query the user using a GUI if the platform supports it.

Observe the Nuget packages included in the project and note which stem from the Microsoft.Bot.Builder namespace. These are essential to building a bot. If you are coming from V3, this project will also help you appreciate that the V4 library allows your bot projects to be far more modular.

### Prerequisites
Before beginning, be sure to have created a QnA Maker knowledge base. Which in turn requires a QnA Maker Service created on your Azure subscription. 

In the project, you will see that a QnA Service has been added to BotConfiguration.bot.

**Be sure to update the KbId, Key, and Endpoint values to match your knowledge base!**

### Steps:
1. Get the top result that meets or exceeds the confidence threshold
2. Create labels from the first question of each result in the response
3. Create buttons for each of the labels. Make these of type ImBack.
4. Create a hero card to display the disambiguation prompt with buttons
5. Create and send an activity with the herocard as an attachment
6. Deserialize from the answer of the topresult

Observe the type we are using to encode our data. This can be whatever you define. It is up to you to choose how to serialize your content.

**Congratulations!** You have enhanced the usability of your QnA Application!