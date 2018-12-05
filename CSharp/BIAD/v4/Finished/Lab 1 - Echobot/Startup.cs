using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BotWorkshop.Bots;

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

            //STEP 3) Add an instance of EchoBot to the services container
            // - Retrieve secret from configuration section "botFileSecret"
            // - Retrieve bot filepath from configuration section "botFilePath"
            // - Create a new instance of BotConfiguration via the static BotConfiguration.Load method
            //   -  (@".\BotConfiguration.bot" can be used as a default)
            // - create a new SimpleCredentialProvider based on the appId/AppPassword passed for the current environment
            //   - (Both can be empty strings for local development)
            // - Add a new instance of ConversationState to the options, based on a new instance of MemoryStorage

            services.AddBot<EchoBot>(options =>
            {
                //Traffic coming through to your app is protected by the app ID and app secret
                //credentials applied to your Bot Registration on Azure.
                //If you are running locally, these can be blank.
                var secretKey = Configuration.GetSection("botFileSecret")?.Value;
                var botFilePath = Configuration.GetSection("botFilePath")?.Value;

                var botConfig = BotConfiguration.Load(botFilePath ?? @".\BotConfiguration.bot", secretKey);
                if (botConfig == null)
                    throw new InvalidOperationException($"The .bot config file could not be loaded from [{botFilePath}]");

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
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            //Step 4) invoke the IApplicationBuilder extension method UseBotFramework() 
            //to automatically map endpoint handlers for bots
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();  //Automatically maps endpoint handlers related to bots
        }
    }
}
