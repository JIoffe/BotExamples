namespace PizzaBot.Responses
{
    public class Orders
    {
        public const string AddressPrompt = "What is the address for this order?";
        public const string AddressReprompt = "I'm sorry, that doesn't look like a valid address. Can you tell me the address for this order?";
        public const string AddressConfirm = "OK, we will ship the order to {0}. Is that correct?";

        public const string SizePrompt = "What size pie would you like?";
        public const string SizeReprompt = "Whoa buddy, that isn't one of our sizes. What size pie can I get for you?";

        public const string ToppingsPrompt = "What toppings would you like?";
        public const string AdditionalToppingsPrompt = "Would you like additional toppings?";
        public const string ToppingsReprompt = "We don't serve {0} - see our menu below!";
        public const string ToppingsDone = "Done with Toppings";

        public const string ConfirmOrder = "Would you like to proceed with this order?";
        public const string CompletedOrder = "Thank you! Enjoy your delicious {0} pie!";
        public const string CancelledOrder = "Fine, be that way.";
    }
}
