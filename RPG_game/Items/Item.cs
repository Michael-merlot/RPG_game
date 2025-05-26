using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }

        public Item(string Name, string Description, int value)
        {
            this.Name = Name;
            this.Description = Description;
            Value = value;
        }
    }

    public class Weapon : Item
    {
        public int Damage { get; private set; }

        public Weapon(string name, string description, int damage, int value) : base(name, description, damage)
        {
            Damage = damage;
        }
    }

    public class Armor : Item
    {
        public int Defense { get; private set; }

        public Armor(string name, string description, int defense, int value) : base(name, description, value)
        {
            Defense = defense;
        }
    }

    public abstract class UsableItem : Item
    {
        public UsableItem(string name, string description, int value) : base (name, description, value)
        {

        }

        public abstract void Use(Player player);
    }

    public class HealthPotion : UsableItem
    {
        public int HealAmount { get; private set; }

        public HealthPotion(string name, string description, int healAmount, int value) : base(name, description, value)
        {
            HealAmount = healAmount;
        }

        public override void Use(Player player)
        {
            int actualHeal = Math.Min(HealAmount, player.MaxHealth - player.Health);
            player.Health += actualHeal;

            Console.WriteLine($"Вы использовали {Name} и восстановили {actualHeal} здоровья");
        }
    }
}
