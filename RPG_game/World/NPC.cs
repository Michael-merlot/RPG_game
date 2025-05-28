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
            int totalCost;

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
                Console.WriteLine("*Медленно и неуверенно уходите от женщины, который провожает вас своим взглядом*");
                Thread.Sleep(1000);
                Console.WriteLine("- Какая жуткая... введьма что ли");
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
                string fullName = $"{tierPrefix}{(name.StartsWith("А") || name.StartsWith("О") || name.StartsWith("У") || name.StartsWith("Э") || name.StartsWith("И") 
                    ? "ый" : "ая")} {name.ToLower()}";

                inventory.Add(new Weapon(fullName, $"Урон: {damage}", damage, value));
            }

            string[] armorNames = { "Кожаная броня", "Кольчуга", "Стальной нагрудник", "Щит", "Шлем", "Поножи", "Наручи" };

            for (int i = 0; i < 3; i++)
            {
                string name = armorNames[random.Next(armorNames.Length)];
                int tier = random.Next(1, 4);
                int defense = 1 + tier * 2 + random.Next(-1, 2);
                int value = 10 + tier * 15 + random.Next(-5, 6);

                string tierPrefix = tier == 1 ? "Обычн" : (tier == 2 ? "Качественн" : "Отличн");
                string fullName = $"{tierPrefix}{(name.StartsWith("А") || name.StartsWith("О") || name.StartsWith("У") || name.StartsWith("Э") || name.StartsWith("И")
                    ? "ый" : "ая")} {name.ToLower()}";

                inventory.Add(new Armor(fullName, $"Защита: {defense}", defense, value));
            }

            string[] potionNames = { "Малое зелье здоровья", "Среднее зелье здоровья", "Большое зелье здоровья" };
            int[] healAmounts = { 30, 60, 100 };
            int[] potionValues = { 15, 30, 50 };

            for (int i = 0; i < 3; i++)
            {
                inventory.Add(new HealthPotion(potionNames[i], $"Восстанавливает {healAmounts[i]} здоровья", healAmounts[i], potionValues[i]));
            }
        }

        public override void Interact(Player player, Game game)
        {
            bool exitTranding = false;

            while (!exitTranding)
            {
                Console.Clear();
                Console.WriteLine($"=== Торговец {Name} ===");
                Console.WriteLine(Description);
                Console.WriteLine($"\nВаше золото: {player.Gold}");

                Console.WriteLine("=== Действия ===");
                Console.WriteLine("1. Купить товары");
                Console.WriteLine("2. Продать предметы");
                Console.WriteLine("0. Выйти");

                Console.Write("\nВаш выбор: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        BuyItems(player);
                        break;
                    case "2":
                        SellItems(player);
                        break;
                    case "0":
                        exitTranding = true;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                        Console.ReadKey(true);
                        break;
                }
            }
        }

        private void SellItems(Player player)
        {
            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("У вас нет предметов для продажи");
                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                Console.ReadKey(true);
                return;
            }

            Console.Clear();
            Console.WriteLine("=== Продажа предметов ===");
            Console.WriteLine($"Ваше золото: ");

            for (int i = 0; i < player.Inventory.Count; i++)
            {
                int sellPrice = player.Inventory[i].Value / 2;
                Console.WriteLine($"{i + 1} - {player.Inventory[i].Name} - {player.Inventory[i].Description} - {player.Inventory[i].Value} золота");
            }

            Console.WriteLine("0. Назад");

            Console.Write("\nВыберите предмет для продажи (номер): ");

            if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex > 0 && selectedIndex <= player.Inventory.Count)
            {
                Item selectedItem = player.Inventory[selectedIndex - 1];
                int sellPrice = selectedItem.Value / 2;

                Console.WriteLine($"Вы уверены, что хотите продать {selectedItem.Name} за {sellPrice} золота? (д/н)");

                if (Console.ReadLine().ToLower().StartsWith("д"))
                {
                    player.Gold += sellPrice;
                    player.Inventory.Remove(selectedItem);

                    Console.WriteLine($"\nВы продали {selectedItem} за {sellPrice} золота!");
                    Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                    Console.ReadKey(true);
                }
            }
        }

        private void BuyItems(Player player)
        {
            bool exitBuying = false;

            while (!exitBuying)
            {
                Console.Clear();
                Console.WriteLine("=== Товары на продажу ===");
                Console.WriteLine($"Ваше золото: {player.Gold}");

                Console.WriteLine($"\n=== Оружие ===");

                int itemIndex = 1;

                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i] is Weapon)
                    {
                        Console.WriteLine($"{itemIndex} - {inventory[i].Name} - {inventory[i].Description} - {inventory[i].Value} золота");
                        itemIndex++;
                    }
                }

                Console.WriteLine("=== Броня ===");

                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i] is Armor)
                    {
                        Console.WriteLine($"{itemIndex} - {inventory[i].Name} - {inventory[i].Description} - {inventory[i].Value} золота");
                        itemIndex++;
                    }
                }

                Console.WriteLine("=== Зелья ===");

                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i] is HealthPotion)
                    {
                        Console.WriteLine($"{itemIndex} - {inventory[i].Name} - {inventory[i].Description} - {inventory[i].Value} золота");
                        itemIndex++;
                    }
                }

                Console.WriteLine("0. Назад");

                Console.Write("\nВыберите товар для покупки (номер): ");

                if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex > 0 && selectedIndex <= inventory.Count)
                {
                    Item selectedItem = null;
                    int counter = 1;

                    for (int i = 0; i < inventory.Count; i++)
                    {
                        if (counter == selectedIndex)
                        {
                            selectedItem = inventory[i];
                            break;
                        }
                        counter++;
                    }

                    if (selectedItem != null)
                    {
                        if (player.Gold >= selectedItem.Value)
                        {
                            Console.WriteLine($"Вы уверены, что хотите купить {selectedItem.Name} за {selectedItem.Value} золота? (д/н)");
                            
                            if (Console.ReadLine().ToLower().StartsWith("д"))
                            {
                                player.Gold -= selectedItem.Value;

                                Item boughtItem;

                                if (selectedItem is Weapon weapon)
                                {
                                    boughtItem = new Weapon(weapon.Name, weapon.Description, weapon.Damage, weapon.Value);
                                }
                                else if (selectedItem is Armor armor)
                                {
                                    boughtItem = new Armor(armor.Name, armor.Description, armor.Defense, armor.Value);
                                }
                                else if (selectedItem is HealthPotion potion)
                                {
                                    boughtItem = new HealthPotion(potion.Name, potion.Description, potion.HealAmount, potion.Value);
                                }
                                else
                                {
                                    boughtItem = new Item(selectedItem.Name, selectedItem.Description, selectedItem.Value);
                                }

                                player.AddItem(boughtItem);
                                Console.WriteLine($"\nВы купили {boughtItem} за {selectedItem.Value} золота");

                                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                                Console.ReadKey(true);
                            }
                        }
                        else
                        {
                            Console.WriteLine("У вас недостаточно золота для этой покупки!");
                            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                            Console.ReadKey(true);
                        }
                    }
                }
                else if (selectedIndex == 0)
                {
                    exitBuying = true;
                }
            }
        }
    }
}
