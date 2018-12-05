# Lab 2.1
## QnA Bot with rich content
Let us take it a step further using rich content! 

Using [QnA Maker](https://www.qnamaker.ai/) we will prototype a bot that can answer some frequently asked questions. QnA Maker is an extremely powerful API that greatly simplifies FAQ or knowledge-base interactions. Furthermore, QnA Maker answers can be *anything* we would like! They can be hyperlinks to files or serialized data. In this case, we will serialize a rich card using JSON.

Observe the Nuget packages included in the project and note which stem from the Microsoft.Bot.Builder namespace. These are essential to building a bot. If you are coming from V3, this project will also help you appreciate that the V4 library allows your bot projects to be far more modular.

### Prerequisites
Before beginning, be sure to have created a QnA Maker knowledge base. Which in turn requires a QnA Maker Service created on your Azure subscription. 

In the project, you will see that a QnA Service has been added to BotConfiguration.bot.

**Be sure to update the KbId, Key, and Endpoint values to match your knowledge base!**

### Steps:
1. Convert the answer to an instance of RichQnAResult
2. Create a hero card based on the content of the richQnaResult
3. Create a new activity that we can attach the hero card to
4. Send the activity to the user

Observe the type we are using to encode our data. This can be whatever you define. It is up to you to choose how to serialize your content.

**Congratulations!** You have added rich content to your QnA Application.