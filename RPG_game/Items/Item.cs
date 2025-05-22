using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public class Item
    {
        public string Name {  get; set; }
        public string Description { get; set; }
        public int Value { get; set; }

        public Item(string Name, string Description, int value)
        {
            this.Name = Name;
            this.Description = Description;
            Value = value;
        }

    }
}
