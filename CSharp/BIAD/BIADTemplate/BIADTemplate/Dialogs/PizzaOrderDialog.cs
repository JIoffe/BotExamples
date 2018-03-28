using System;
using System.Threading.Tasks;
using BIADTemplate.Dialogs.Prompts;
using BIADTemplate.Model;
using Microsoft.Bot.Builder.Dialogs;

namespace BIADTemplate.Dialogs
{
    [Serializable]
    public class PizzaOrderDialog : IDialog<string>
    {
        private PizzaOrder _order;

        public async Task StartAsync(IDialogContext context)
        {
            _order = new PizzaOrder();

            await context.PostAsync("ok let me get your order...");
            PromptDialog.Text(context, ResumeAfter_AddressPrompt, "What is the address?");
        }

        private async Task ResumeAfter_AddressPrompt(IDialogContext context, IAwaitable<string> result)
        {
            var address = await result;
            await context.PostAsync($"Alright, I got {address}. Whew, what a neighborhood!");
            _order.Address = address;

            context.Call(new PizzaToppingPrompt(), ResumeAfter_PizzaToppingPrompt);
        }
        
        private async Task ResumeAfter_PizzaToppingPrompt(IDialogContext context, IAwaitable<PizzaOrder.PizzaTopping> result)
        {
            var topping = await result;
            await context.PostAsync($"Alright, {topping.ToString().ToLowerInvariant()} it is.");
            _order.Topping = topping;

            context.Call(new PieSizePrompt(), ResumeAfter_PieSizePrompt);
        }

        private async Task ResumeAfter_PieSizePrompt(IDialogContext context, IAwaitable<PizzaOrder.PieSize> result)
        {
            var size = await result;
            _order.Size = size;

            await context.PostAsync($"Alright, a delicious {size.ToString().ToLowerInvariant()} {_order.Topping.ToString().ToLowerInvariant()} pie is coming your way.");
            await context.PostAsync($"It should be at {_order.Address} within the next 15 minutes or your money back!");

            context.Done(string.Empty);
        }
    }
}