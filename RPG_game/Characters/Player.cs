using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public class Player
    {
        public string Name {  get; set; }
        public int  Health { get; set; }
        public int MaxHealth { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Gold { get; set; }
        public List<Item> Inventory { get; set; }

        public Player(string name, int health, int level)
        {
            Name = name;
            MaxHealth = health;
            Health = health;
            Level = level;
            Experience = 0;
            Gold = 10;
            Inventory = new List<Item>();
        }

        public void AddExperience(int amount)
        {
            Experience += amount;

            if (Experience >= Level * 100)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Level++;
            MaxHealth += 10;
            Health = MaxHealth;
            Console.WriteLine($"Поздравляем! Вы достигли уровня: {Level}");
            Console.WriteLine($"Ваше максимальное здоровье равняется: {MaxHealth}");
        }

        public void AddItem(Item item)
        {
            Inventory.Add(item);
            Console.WriteLine($"Получен предмет: {item.Name}");
        }
    }
}
