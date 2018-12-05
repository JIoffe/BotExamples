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

                //var secretKey = ???
                //var botFilePath = ???

                //TODO - use BotConfiguration.Load to retrieve an instance of a BotConfiguration

                //The bot configuration can map different endpoints for different environments
                var serviceName = Environment.EnvironmentName.ToLowerInvariant();

                //TODO - Retrieve the service from the bot configuration by name or ID

                //TODO - Set options.CredentialProvider to a new SimpleCredentialProvider based on this service

                //TODO - Create an instance of MemoryStorage

                //TODO - Add a new ConversationState to the options state list
                //     - Have it reference the MemoryStorage
                //(You would not use MemoryStorage in a production application!)
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
