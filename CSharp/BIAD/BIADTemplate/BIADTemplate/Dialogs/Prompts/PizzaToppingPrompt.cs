using BIADTemplate.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BIADTemplate.Dialogs.Prompts
{
    [Serializable]
    public class PizzaToppingPrompt: IDialog<PizzaOrder.PizzaTopping>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var msg = context.MakeMessage();

            msg.Text = "Which topping would you like?";
            msg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            msg.Attachments = GetToppingCards();

            await context.PostAsync(msg);
            context.Wait(ToppingSelected);
        }

        private async Task ToppingSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
            var response = activity.Value as string ?? activity.Text;

            var pizzaTopping = Enum.Parse(typeof(PizzaOrder.PizzaTopping), response);

            context.Done(pizzaTopping);
        }

        public IList<Attachment> GetToppingCards()
        {
            return new HeroCard[]
            {
                new HeroCard
                {
                    Title = "Plain",
                    Text = "Simple but delicious. Unadulterated cheese",
                    Images = new CardImage[]
                    {
                        new CardImage("https://cdn.pixabay.com/photo/2017/12/05/20/09/pizza-3000274_1280.jpg")
                    },
                    Buttons = new CardAction[]
                    {
                        new CardAction(ActionTypes.PostBack, "Plain", value: "PLAIN")
                    }
                },
                new HeroCard
                {
                    Title = "Pepperoni",
                    Text = "So greasy but so satisfying",
                    Images = new CardImage[]
                    {
                        new CardImage("https://cdn.pixabay.com/photo/2017/10/17/00/56/pizza-2859233_1280.jpg")
                    },
                    Buttons = new CardAction[]
                    {
                        new CardAction(ActionTypes.PostBack, "Pepperoni", value: "PEPPERONI")
                    }
                },
                new HeroCard
                {
                    Title = "Mushroom",
                    Text = "Slightly healthier than pepperoni",
                    Images = new CardImage[]
                    {
                        new CardImage("https://cdn.pixabay.com/photo/2014/07/08/12/34/pizza-386717_1280.jpg")
                    },
                    Buttons = new CardAction[]
                    {
                        new CardAction(ActionTypes.PostBack, "Mushroom", value: "MUSHROOM")
                    }
                },
                new HeroCard
                {
                    Title = "Sausage",
                    Text = "Slightly greasier than pepperoni",
                    Images = new CardImage[]
                    {
                        new CardImage("https://cdn.pixabay.com/photo/2018/03/06/13/48/food-3203448_1280.jpg")
                    },
                    Buttons = new CardAction[]
                    {
                        new CardAction(ActionTypes.PostBack, "Sausage", value: "SAUSAGE")
                    }
                },
                new HeroCard
                {
                    Title = "Veggie",
                    Text = "Who are you trying to kid? This is pizza!",
                    Images = new CardImage[]
                    {
                        new CardImage("https://cdn.pixabay.com/photo/2014/05/18/11/25/pizza-346985_1280.jpg")
                    },
                    Buttons = new CardAction[]
                    {
                        new CardAction(ActionTypes.PostBack, "Veggie", value: "VEGGIE")
                    }
                },
                new HeroCard
                {
                    Title = "Buffalo",
                    Text = "You can't get enough of these",
                    Images = new CardImage[]
                    {
                        new CardImage("https://cdn.pixabay.com/photo/2017/12/05/20/08/pizza-3000282_1280.png")
                    },
                    Buttons = new CardAction[]
                    {
                        new CardAction(ActionTypes.PostBack, "Buffalo", value: "BUFFALO")
                    }
                }
            }
            .Select(card => card.ToAttachment())
            .ToList();
        }
    }
}