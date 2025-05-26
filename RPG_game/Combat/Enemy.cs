using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public class Enemy
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int AttackPower { get; set; }
        public int Defense { get; set; }
        public int DefenseBonus { get; set; }
        public int Level { get; private set; }
        public int ExpReward { get; set; }
        public int GoldReward { get; set; }

        public Enemy(string name, string description, int level)
        {
            Name = name;
            Description = description;
            Level = level;

            MaxHealth = 20 + level * 10;
            Health = MaxHealth;

            AttackPower = 5 + level * 2;
            Defense = 2 + level;

            ExpReward = 10 + level * 5;
            GoldReward = 5 + level * 3;

            DefenseBonus = 0;
        }


        public int GetAttackDamage()
        {
            return AttackPower;
        }
    }
}
