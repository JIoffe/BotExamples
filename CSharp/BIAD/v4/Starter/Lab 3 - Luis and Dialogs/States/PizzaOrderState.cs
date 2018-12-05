using JIoffe.PizzaBot.Model;

namespace PizzaBot.States
{
    /// <summary>
    /// Persistent state of a pizza order per user per conversation
    /// </summary>
    public class PizzaOrderState
    {
        public string CustomerName { get; set; }
        public string PreferredAddress { get; set; }
        public PizzaOrder CurrentOrder { get; set; }
    }
}
