using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public class Location
    {
        public string Name {  get; private set; }
        public string Description { get; private set; }
        public List<Location> Neighbors { get; private set; }

        public Location(string name, string description)
        {
            Name = name;
            Description = description;
            Neighbors = new List<Location>();
        }

        public void AddNeighbor(Location location)
        {
            if (!Neighbors.Contains(location))
            {
                Neighbors.Add(location);
            }
        }
    }
}
