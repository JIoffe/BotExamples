using Microsoft.Bot.Builder.Dialogs;

namespace PizzaBot.Dialogs
{
    /// <summary>
    /// Simple interface for producing Dialogs we can add to a DialogStep
    /// </summary>
    public interface IDialogFactory
    {
        Dialog Build();
    }
}
