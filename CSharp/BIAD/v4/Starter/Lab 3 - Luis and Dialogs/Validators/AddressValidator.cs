using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace PizzaBot.Validators
{
    public class AddressValidator
    {
        public static async Task<bool> ValidateAddressAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            //Not the most elaborate validation method...
            //All's bob if the string is not empty.
            var isValid = promptContext.Recognized.Succeeded && !string.IsNullOrWhiteSpace(promptContext.Recognized.Value);
            if (!isValid)
            {
                await promptContext.Context.SendActivityAsync(Responses.Orders.AddressReprompt);
            }

            return isValid;
        }
    }
}
