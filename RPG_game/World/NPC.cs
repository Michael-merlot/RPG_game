using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public abstract class NPC
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }

        public NPC(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public abstract void Interact(Player player, Game game);
    }

    public class Healer : NPC
    {
        private int healCost;

        public Healer(string name, string description, int healCost) : base (name, description) 
        {
            this.healCost = healCost;
        }

        public override void Interact(Player player, Game game)
        {
            Console.Clear();
            Console.WriteLine($"=== {Name} ===");
            Console.WriteLine(Description);

            int healAmount = player.MaxHealth - player.Health;
            int totalCost = 0;

            if (healCost > 0)
            {
                totalCost = (int)Math.Ceiling((double)healAmount / 10) * healCost;

                Console.WriteLine($"\n\"Здрасьте, я могу восстановить ваше здоровье за {totalCost} золота.\"");
                Console.WriteLine($"Текущее здоровье: {player.Health}/{player.MaxHealth}");
                Console.WriteLine($"У вас есть: {player.Gold} золота");

                if (player.Gold >= totalCost)
                {
                    Console.WriteLine("\n1. Вылечиться полностью");
                    Console.WriteLine("2. Вылечиться частично");
                    Console.WriteLine("0. Отказаться");

                    Console.Write("\nВаш выбор: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            player.Gold -= totalCost;
                            player.Health = player.MaxHealth;
                            Console.WriteLine($"Вы принимаете странное зелье, которое пахнет как... удача?");
                            Thread.Sleep(2000);
                            Console.WriteLine($"\"Вот так-то лучше! Вы полностью здоровы.\"");
                            Thread.Sleep(2000);
                            Console.WriteLine($"Здоровье восстановлено до {player.Health}/{player.MaxHealth}");
                            break;

                        case "2":
                            PartialHealing(player);
                            break;

                        default:
                            Console.WriteLine("\"Возвращайтесь, если понадоблюсь.\"");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\n\"Мне жаль, но у вас недостаточно золота. Возвращайтесь, когда накопите больше.\"");
                }
            }
            else
            {
                Console.WriteLine("\n\"Вы полностью здоровы, вам не нужно моё лечение.\"");
                Thread.Sleep(1000);
                Console.WriteLine("- А вы откуда знаете?");
                Thread.Sleep(1000);
                Console.WriteLine("\n\"Над твоим лицом панелька висит, приглядись.\"");
                Thread.Sleep(1000);
                Console.WriteLine("*Вы медленно смотрите на вверх и ничего не видите*");
                Thread.Sleep(1000);
                Console.WriteLine("- Но я ничего не вижу... что здесь вообще происходит?");
                Thread.Sleep(1000);
                Console.WriteLine("\n\"Всему своё время, странник.\"");
                Thread.Sleep(1000);
                Console.WriteLine("*Медленно и неуверенно уходите от мужчины, который провожает вас своим взглядом*");
                Thread.Sleep(1000);
                Console.WriteLine("- Какой жуткий тип");
                Thread.Sleep(1000);

                Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
                Console.ReadKey();
            }
        }

        private void PartialHealing(Player player)
        {
            Console.Clear();
            Console.WriteLine($"=== {Name} ===");
            Console.WriteLine($"Текущее здоровье: {player.Health}/{player.MaxHealth}");
            Console.WriteLine($"Стоимость лечения: {healCost} золота за 10 единиц здоровья");
            Console.WriteLine($"У вас есть: {player.Gold} золота");

            Console.Write("\nСколько единиц здоровья вы хотите восстановить (кратно 10): ");
            if (int.TryParse(Console.ReadLine(), out int value) && value > 0)
            {
                value = (int)Math.Ceiling(value / 10.0) * 10;
                value = Math.Min(value, player.MaxHealth - player.Health);

                int cost = (value / 10) * healCost;

                if (player.Gold >= cost && value > 0)
                {
                    player.Gold -= cost;
                    player.Health += value;

                    Console.WriteLine($"\"Вот так-то лучше! Вы полностью здоровы.\"");
                    Thread.Sleep(1000);
                    Console.WriteLine($"Здоровье восстановлено до {player.Health}/{player.MaxHealth}");
                }
                else
                {
                    if (value <= 0)
                    {
                        Console.WriteLine("Вам не нужно лечение");
                    }
                    else
                    {
                        Console.WriteLine("У вас недостаточно золота для такого лечения");
                    }
                }
            }
            else
            {
                Console.WriteLine("Неверное количество. Лечение отменено");
            }
        }
    }

    public class Trader : NPC
    {
        private List<Item> inventory;

        public Trader(string name, string description) : base(name, description)
        {
            inventory = new List<Item>();
            GenerateInventory();
        }

        private void GenerateInventory()
        {
            Random random = new Random();

            string[] weaponNames = { "Кинжал", "Короткий меч", "Длинный меч", "Топор", "Булава", "Боевой молот", "Копье", "Боевой лук" };

            for (int i = 0; i < 3; i++)
            {
                string name = weaponNames[random.Next(weaponNames.Length)];
                int tier = random.Next(1, 4);
                int damage = 3 + tier * 3 + random.Next(-1, 2);
                int value = 10 + tier * 20 + random.Next(-5, 6);

                string tierPrefix = tier == 1 ? "Обычн" : (tier == 2 ? "Качественн" : "Отличн");
                string fullName = $"{tierPrefix}{(name.StartsWith("А") || name.StartsWith("А") || name.StartsWith("А") || name.StartsWith("А") || name.StartsWith("А") 
                    ? "ый" : "ая")} {name.ToLower()}";

                inventory.Add(new Weapon(fullName, $"Урон: {damage}", damage, value));
            }
        }
    }
}
