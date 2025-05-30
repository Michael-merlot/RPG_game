using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public class Boss : Enemy
    {
        public string Title { get; private set; }
        public string IntroText { get; private set; }
        public string DefeatText { get; private set; }
        public List<Item> GuaranteedLoot { get; private set; }
        public bool IsDefeated { get; private set; }
        public int SpecialAttackChance { get; private set; }
        public int PhaceCount { get; private set; }
        public int CurrentPhase { get; private set; }

        public Boss(string name, string description, string title, int level) : base(name, description, level)
        {
            Title = title;
            IntroText = $"Перед вами предстает {Title} {Name}! Приготовьтесь!";
            DefeatText = $"{Title} {Name} повержен! Вы выиграли эту битву!";
            GuaranteedLoot = new List<Item>();
            IsDefeated = false;
            SpecialAttackChance = 30;
            PhaceCount = 1;
            CurrentPhase = 1;

            MaxHealth = (int)(MaxHealth * 2.5);
            Health = MaxHealth;
            AttackPower = (int)(AttackPower * 1.8);
            Defense = (int)(Defense * 1.5);

            ExpReward *= 3;
            GoldReward *= 4;
        }
        public void AddGuaranteedLoot(Item item)
        {
                GuaranteedLoot.Add(item);
        }
        public int PerformSpecialAttack()
        {
            Random random = new Random();
            int attackType = random.Next(3);
            int baseDamage = GetAttackDamage();

            switch (attackType)
            {
                case 0: // мощный удар
                    int strongAttackDamage = (int)(baseDamage * 1.5);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n{Name} наносит МОЩНЫЙ удар, нанося {strongAttackDamage} урона!");
                    Console.ResetColor();
                    return strongAttackDamage;

                case 1: // двойная атака
                    int firstHit = (int)(baseDamage * 0.7);
                    int secondHit = (int)(baseDamage * 0.7);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n{Name} наносит ДВОЙНУЮ атаку! Первый удар: {firstHit}, второй удар: {secondHit}");
                    Console.ResetColor();
                    return firstHit + secondHit;

                case 2: // восстановление и атака
                    int healAmount = MaxHealth / 10;
                    Health = Math.Min(MaxHealth, Health + healAmount);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n{Name} использует ВОССТАНОВЛЕНИЕ, исцеляясь на {healAmount} здоровья!");
                    Console.WriteLine($"\n{Name} также атакует вас, нанося {baseDamage} урона!");
                    Console.ResetColor();
                    return baseDamage;

                default:
                    return baseDamage;
            }
        }
        public void NextPhase()
        {
            if (CurrentPhase < PhaceCount)
            {
                CurrentPhase++;

                int healAmount = MaxHealth / 3;
                Health = Math.Min(MaxHealth, Health + healAmount);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\n{Name} переходит к ФАЗЕ {CurrentPhase}! Восстановлено {healAmount} здоровья!");

                AttackPower = (int)(AttackPower * 1.2);
                Defense = (int)(Defense * 1.1);
                Console.WriteLine($"Атака босса увеличена до {AttackPower}!");
                Console.WriteLine($"Защита босса увеличена до {Defense}!");
                Console.ResetColor();
            }
        }
        public bool ShouldChangePhase() // метод для проверки, надо ли переходить к след. фазе (если 33% здоровья или меньше - фаза меняется)
        {
            float healthPercentage = (float)Health / MaxHealth * 100;
            return CurrentPhase < PhaceCount && healthPercentage <= 33;
        }
        public void Defeat()
        {
            IsDefeated = true;
        }
    }
}
