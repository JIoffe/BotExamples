using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BIADTemplate.Model
{
    [Serializable]
    public class PizzaOrder
    {
        public enum PizzaTopping
        {
            PLAIN, PEPPERONI, MUSHROOM, SAUSAGE, VEGGIE, BUFFALO
        }

        public enum PieSize
        {
            SMALL, MEDIUM, LARGE
        }

        public string Address { get; set; }
        public PieSize Size { get; set; }
        public PizzaTopping Topping { get; set; }


        public static IForm<PizzaOrder> BuildOrderForm()
        {
            return new FormBuilder<PizzaOrder>()
                .Field(nameof(Address), "What's your address?")
                .Build();
        }
    }
}