using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JIoffe.PizzaBot.Model
{
    /// <summary>
    /// Represents an active order for a particular user.
    /// </summary>
    public class PizzaOrder
    {
        public string Address { get; set; }
        public ISet<PizzaTopping> Toppings { get; set; }
        public PizzaSize Size { get; set; }
    }
}
