using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PizzaBot.Accessors;
using PizzaBot.Bots;
using PizzaBot.Dialogs;
using PizzaBot.States;

namespace JIoffe.PizzaBot
{
    public class Startup
    {
        private ILoggerFactory _loggerFactory;
        public IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; } 

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            Environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //The extension to add a bot to our services container
            //can be found in Microsoft.Bot.Builder.Integration.AspNet.Core
            //Whichever type is passed will be the bot that is created
            services.AddBot<PizzaOrderBot>(options =>
            {
                //Traffic coming through to your app is protected by the app ID and app secret
                //credentials applied to your Bot Registration on Azure.
                //If you are running locally, these can be blank.
                var secretKey = Configuration.GetSection("botFileSecret")?.Value;
                var botFilePath = Configuration.GetSection("botFilePath")?.Value;

                var botConfig = BotConfiguration.Load(botFilePath ?? @".\BotConfiguration.bot", secretKey);
                if (botConfig == null)
                    throw new InvalidOperationException($"The .bot config file could not be loaded from [{botFilePath}]");

                //Add the bot configuration as something we can retrieve through DI
                services.AddSingleton(sp => botConfig);

                //The bot configuration can map different endpoints for different environments
                var serviceName = Environment.EnvironmentName.ToLowerInvariant();
                var service = botConfig.FindServiceByNameOrId(serviceName) as EndpointService;
                if(service == null)
                    throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{serviceName}'.");

                options.CredentialProvider = new SimpleCredentialProvider(service.AppId, service.AppPassword);


                //Memory storage is only used for this example.
                //In a production application, you should always rely on 
                //a more persistant storage mechanism, such as CosmosDB
                IStorage dataStore = new MemoryStorage();

                //Whichever datastore we're working with, we will need to use it
                //to actually store the conversation state.
                var conversationState = new ConversationState(dataStore);
                options.State.Add(conversationState);

                //Setup a call back to use in case any application or middleware exceptions were thrown
                ILogger logger = _loggerFactory.CreateLogger<EchoBot>();

                options.OnTurnError = async (context, e) =>
                {
                    //Ideally you do not want to get here - you want to handle business logic errors more gracefully.
                    //But if we're here, we can do any housekeeping such as logging and letting the know something went wrong.
                    logger.LogError(e, "Unhandled exception on bot turn - incoming input was {0}", context.Activity.Text);

                    //Since we have the context, we can send (helpful) messagse to the user
                    await context.SendActivityAsync($"Something went wrong. Please forgive me. Exception: {e.Message}");
                };
            });

            //Any state accessors need to be injected into the bot for each turn,
            //so add them to the services container
            services.AddSingleton<PizzaOrderAccessors>(sp =>
            {
                //Step 24) Retrieve the bot options from the IServiceCollection.
                //This is found via the type IOptions<BotFrameworkOptions>
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                if (options == null)
                {
                    throw new InvalidOperationException("BotFrameworkOptions must be configured prior to setting up the state accessors");
                }

                //Step 25) Retrieve the ConversationState from the options.
                var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
                if (conversationState == null)
                {
                    throw new InvalidOperationException("ConversationState must be defined and added before adding conversation-scoped state accessors.");
                }

                //Step 26) Now our factory method needs to return a new instance of type PizzaOrderAccessors
                //based on this conversation state. We also need to register new properties for each
                //state we're tracking - the "PizzaOrderState" and the "DialogState". 
                var accessors = new PizzaOrderAccessors(conversationState)
                {
                    PizzaOrderState = conversationState.CreateProperty<PizzaOrderState>("PizzaOrderState"),
                    DialogState = conversationState.CreateProperty<DialogState>("DialogState")
                };

                return accessors;
            });

            //Step 27 )Add a singleton of type LuisRecognizer to the services container.
            //This involves creating a new instance of LuisApplication and LuisPredictionOptions.
            services.AddSingleton(sp =>
            {
                var appId = Configuration.GetSection("luisAppId")?.Value;
                var key = Configuration.GetSection("luisKey")?.Value;
                var endpoint = Configuration.GetSection("luisEndpoint")?.Value;

                // Get LUIS information
                var luisApp = new LuisApplication(appId, key, endpoint);

                // Specify LUIS options. These may vary for your bot.
                var luisPredictionOptions = new LuisPredictionOptions
                {
                    IncludeAllIntents = true,
                };

                // Create the recognizer
                var recognizer = new LuisRecognizer(luisApp, luisPredictionOptions, true, null);
                return recognizer;
            });

            //Step 28) The last setup step for this lab. Add singletons for "MainMenuDialogFactory" 
            //and "PizzaOrderDialogFactory" to the services container.
            //This shows that they can be decoupled from the bot for reuse and testing.
            services.AddSingleton<MainMenuDialogFactory>();
            services.AddSingleton<PizzaOrderDialogFactory>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();  //Automatically maps endpoint handlers related to bots
        }
    }
}
