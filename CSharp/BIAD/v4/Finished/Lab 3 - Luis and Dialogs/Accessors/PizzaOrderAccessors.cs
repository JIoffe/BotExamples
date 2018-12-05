using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using PizzaBot.States;

namespace PizzaBot.Accessors
{
    //Accessors are meant to provide access to persistent states in a conversation.
    //These include:
    //- The current point in the dialog stack
    //- Other persistent conversation states

    /// <summary>
    /// Provides accessors to the conversation-level states related to the pizza order bot
    /// </summary>
    public class PizzaOrderAccessors
    {
        /// <summary>
        /// References the most current snapshot of the conversation with a particular user
        /// </summary>
        public ConversationState ConversationState { get; }
        /// <summary>
        /// Refers to the current dialog stack - must be re-accessed and updated on each turn!
        /// </summary>
        public IStatePropertyAccessor<DialogState> DialogState { get; set; }
        /// <summary>
        /// Persistent state related to making a pizza order
        /// </summary>
        public IStatePropertyAccessor<PizzaOrderState> PizzaOrderState { get; set; }

        public PizzaOrderAccessors(ConversationState conversationState)
        {
            ConversationState = conversationState;
        }
    }
}
