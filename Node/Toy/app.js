///////////////////////////////////////////
//   Introductory Toy Sample
//-----------------------------
//   A simple bot that can tell the time,
//   the date, or say hello.
//
//   Uses LUIS for intents. Luis endpoint may change in the future.
////////////////////////////////////////////

//Load .env files - this is where all "process.env" variables come from
require('dotenv-extended').load();

//This is all we need to reference the bot framework in node!
var builder = require('botbuilder');
//Restify is a library that makes it easier to handle web requests
var restify = require('restify');

//This allows us to connect to our bot and process messages
var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});

//This is the "bot" with a default failure message
var bot = new builder.UniversalBot(connector, function (session) {
    session.send("Sorry, I didn't understand that!", session.message.text);
});

//Attach a LUIS recognizer - the model URL can be seen on the luis website
var recognizer = new builder.LuisRecognizer(process.env.LUIS_MODEL_URL);
bot.recognizer(recognizer);

//Setup a server to listen for "/api/messages" POST requests
// and pass through to the bot framework's connector
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log('Bot is listening on %s', server.url);
});
server.post('/api/messages', connector.listen());

//Store some information about the user
var name = null;

// GREETING - Hello, how are you, etc. 
bot.dialog('Greeting',[
    function (session, args, next) {
        if(name != null){
            session.send(`Hello ${name}, it's great to see you again!`, session.message.text);
            return;       
        }

        session.send('Hello!', session.message.text);
        builder.Prompts.text(session, 'What is your name?');
    },
    function (session, results){
        name = results.response;
        session.send(`Your name is ${name}? So cool! I will remember that.`, session.message.text);
    }
]).triggerAction({
    matches: 'Greeting'
});

// TIME - What time is it? etc.
bot.dialog('Time', function(session){
    var now = new Date();

    //Convert to 12 hour time
    var hours = now.getHours();
    var minutes = now.getMinutes();

    var abbreviation;
    if(hours >= 12)
        abbreviation = "PM";
    else
        abbreviation = "AM";

    //Convert to 12 hour time
    hours = hours % 12;
    if(hours === 0)
        hours = 12;

    session.send(`Ding dong! The time is now ${hours}:${minutes} ${abbreviation}`, session.message.text);
}).triggerAction({
    matches: 'Time'
});

// DATE - What is today's date?
bot.dialog('Date', function(session){
    var now = new Date();
    session.send(`Today is ${now.toDateString()}.`, session.message.text);
}).triggerAction({
    matches: 'Date'
});