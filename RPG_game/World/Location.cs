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
        public bool IsSafeZone { get; private set; }
        public List<NPC> NPCs { get; private set; }


        public Location(string name, string description, bool isSafeZone = false)
        {
            Name = name;
            Description = description;
            IsSafeZone = isSafeZone;
            Neighbors = new List<Location>();
            NPCs = new List<NPC>();
        }

        public void AddNeighbor(Location location)
        {
            if (!Neighbors.Contains(location))
            {
                Neighbors.Add(location);
            }
        }

        public void AddNPC(NPC npc)
        {
            NPCs.Add(npc);
        }
    }
}
