using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public class Game
    {
        private Player player;
        private bool isRunning;
        private List<Location> world;
        private Location currentLocation;
        public Game()
        {
            isRunning = false;
            world = new List<Location>();
            InitializeGame();
        }

        private void InitializeGame()
        {
            Location village = new Location("Деревня", "Небольшая мирная деревушка. Здесь вы можете отдохнуть и набраться сил.", true);
            Location forest = new Location("Лес", "Густой темный лес. Говорят, здесь водятся опасные существа.");
            Location cave = new Location("Пещера", "Загадочная пещера в горах. Кто знает, какие сокровище скрыты внутри?");
            Location dungeon = new Location("Подземелье", "Древние руины под землей. Говорят, что здесь можно найти ценные артефакты, но и опасные враги обитают тут.");

            world.Add(village);
            world.Add(forest);
            world.Add(cave);
            world.Add(dungeon);

            village.AddNeighbor(forest);
            forest.AddNeighbor(village);
            forest.AddNeighbor(cave);
            cave.AddNeighbor(forest);
            cave.AddNeighbor(dungeon);
            dungeon.AddNeighbor(cave);

            village.AddNPC(new Healer("Травница Елена", "Добрая женщина, которая может вылечить ваши раны за скромную плату.", 5));
            village.AddNPC(new Trader("Кузнец Торим", "Крепкий мужчина с густой бородой. Торгует оружием и доспехами собственного изготовления."));
            village.AddNPC(new Trader("Алхимик Маркус", "Странновый старик с блестящими глазами. Продает различные зелья и эликсиры."));

            currentLocation = village;
        }

        public void Start()
        {
            CreateCharacter();

            isRunning = true;
            GameLoop();
        }

        private void CreateCharacter()
        {
            Console.Clear();
            Console.WriteLine("=== Создание Персонажа ===");
            Console.Write("Введите имя персонажа: ");
            string name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Безымянный спутник"; // ну или Келус :)
            }

            player = new Player(name, 100, 1);

            Console.WriteLine($"Здравствуй, {name}! Похоже... ты здесь впервые, я прав? " +
                $"\nСмотри, до тебя бывали такие же ребята, как и ты, но всех ждало только одно - смерть"
                + $"\nПоэтому будь осторожен, может, тебе удастся дойти до... пх... пх... *связь прервана*");

            Thread.Sleep(8000);

            Console.Write($"*Вы просыпаетесь на тропинке недалеко от Деревни*\n- Может следует туда сходить?");
            Console.WriteLine();
            Console.WriteLine("Нажмите для продолжения...");
            Console.ReadKey(true);
        }

        private void GameLoop()
        {
            while (isRunning)
            {
                DisplayCurrnetLocation();
                string command = GetUserInput();
                ProcessCommand(command);

                if (player.Health <= 0 )
                {
                    Console.Clear();
                    Console.WriteLine($"Вы погибли! Игра окончена...");
                    isRunning = false;
                    Console.ReadKey(true);
                }

            }
        }

        private void DisplayCurrnetLocation()
        {
            Console.Clear();
            Console.WriteLine($"=== {currentLocation.Name} ===");
            Console.WriteLine(currentLocation.Description);
            Console.WriteLine("=======================================");

            Console.WriteLine("\nДоступные пути: ");
            foreach (Location neighbor in currentLocation.Neighbors)
            {
                Console.WriteLine($"- {neighbor.Name}");
            }

            Console.WriteLine("\n--------------------------------------------");
            Console.WriteLine($"{player.Name} | Уровень: {player.Level} | Здоровье: {player.Health}/{player.MaxHealth}");
            Console.WriteLine("\nДействия: [исследовать], [путешествовать], [инвентарь], [выход]");

        }

        public string GetUserInput()
        {
            Console.Write("\nВаше действие: ");
            return Console.ReadLine().ToLower();
        }

        private void ProcessCommand(string command)
        {
            switch (command)
            {
                case "исследовать":
                    Explore();
                    break;

                case "путешествовать":
                    Travel();
                    break;

                case "инвентарь":
                    ShowInventory();
                    break;

                case "выход":
                    isRunning = false;
                    break;

                case "help":
                    Console.WriteLine("Команды: \n- Исследовать\n- Путешествовать\n- Инвентарь\n- Выход");
                    break;

                default:
                    Console.WriteLine("Неизвестная команда. Попробуйте написать help для показа всех команд");
                    Console.ReadKey(true);
                    break;
            }
        }

        private void Explore()
        {
            Console.Clear();
            Console.WriteLine($"Вы исследуете: {currentLocation.Name}");

            Random random = new Random();
            int eventType = random.Next(10);

            switch (eventType)
            {
                case 0:
                case 1:
                case 2:
                    Console.WriteLine("Вы ничего не нашли здесь");
                    break;
                case 3:
                case 4:
                    int gold = random.Next(1, 10);
                    player.Gold = gold;
                    Console.WriteLine($"Вы нашли: {gold} золота!");
                    break;
                case 5:
                    Item foundItem = GenerateRandomItem();
                    player.AddItem(foundItem);
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                    Enemy enemy = EnemyFactory.CreateRandomEnemy(currentLocation.Name, player.Level);
                    Console.WriteLine($"Вы столкнулись с врагом: {enemy.Name}!");
                    Console.WriteLine("Нажмите любую клавишу, чтобы начать бой...");
                    Console.ReadKey(true);

                    CombatSystem combatSystem = new CombatSystem();
                    bool playerWon = combatSystem.StartCombat(player, enemy);

                    if (!playerWon)
                    {
                        isRunning = false;
                        return;
                    }

                    break;
            }

            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private void Travel()
        {
            Console.Clear();
            Console.WriteLine("Куда вы хотите отправиться?");

            for (int i = 0; i < currentLocation.Neighbors.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {currentLocation.Neighbors[i].Name}");
            }

            Console.WriteLine("0. Вернуться");
            Console.Write("\nВаш выбор: ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= currentLocation.Neighbors.Count)
            {
                currentLocation = currentLocation.Neighbors[choice - 1];
                Console.WriteLine($"Вы отправляетесь на локацию: {currentLocation.Name}...");
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey(true);
            }
        }

        private void ShowInventory()
        {
            bool exitInventory = false;

            while (!exitInventory)
            {
                Console.Clear();
                Console.WriteLine("=== Инвентарь и Экипировка ===");

                Console.WriteLine("\nТекущее снаряжение: ");
                Console.WriteLine($"Оружие: {player.EquippedWeapon.Name} (Урон: {player.EquippedWeapon.Damage})");
                Console.WriteLine($"Броня: {player.EquippedArmor.Name} (Защита: {player.EquippedArmor.Defense})");

                Console.WriteLine("\nХарактеристики:");
                Console.WriteLine($"Сила: {player.Strength}");
                Console.WriteLine($"Ловкость: {player.Dexterity}");
                Console.WriteLine($"Телосложение: {player.Constitution}");

                Console.WriteLine($"\nИтоговый урон: {player.GetAttackDamage()}");
                Console.WriteLine($"Итоговая защита: {player.GetDefense()}");

                Console.WriteLine($"Золото: {player.Gold}");

                Console.WriteLine("\nПредметы: ");

                if (player.Inventory.Count == 0)
                {
                    Console.WriteLine("Инвентарь пуст");
                }
                else
                {
                    for (int i = 0; i < player.Inventory.Count; i++)
                    {
                        string itemType = player.Inventory[i] is Weapon ? "[Оружие]" :
                            player.Inventory[i] is Armor ? "[Броня]" :
                            player.Inventory[i] is UsableItem ? "[Предмет]" : "";

                        Console.WriteLine($"{i + 1} - {itemType} {player.Inventory[i].Name} - {player.Inventory[i].Description}");
                    }

                    Console.WriteLine($"\nДействия: [и]спользовать, [э]кипировать, [в]ыход");
                    Console.Write("\nВыберите действие (или номер предмета для подробностей): ");

                    string input = Console.ReadLine().ToLower();

                    if (input == "и" || input == "использовать")
                    {
                        UseItem();
                    }
                    else if (input == "э" || input == "экипировать")
                    {
                        EquipItem();
                    }
                    else if (input == "в" || input == "выход")
                    {
                        exitInventory = true;
                    }
                    else if (int.TryParse(input, out int itemIndex) && itemIndex > 0 && itemIndex <= player.Inventory.Count)
                    {
                        ShowItemDetails(player.Inventory[itemIndex - 1]);
                    }
                    else
                    {
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                        Console.ReadKey(true);
                    }
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private void ShowItemDetails(Item item)
        {
            Console.Clear();
            Console.WriteLine($"=== {item.Name} ===");
            Console.WriteLine($"Описание: {item.Description}");
            Console.WriteLine($"Стоимость: {item.Value} золота");

            if (item is Weapon weapon)
            {
                Console.WriteLine($"Тип: Оружие");
                Console.WriteLine($"Урон: {weapon.Damage}");
                Console.WriteLine($"\nТекущее оружие: {player.EquippedWeapon.Name} (Урон: {player.EquippedWeapon.Damage})");
            }
            else if (item is Armor armor)
            {
                Console.WriteLine($"Тип: Броня");
                Console.WriteLine($"Защита: {armor.Defense}");
                Console.WriteLine($"\nТекущая броня: {player.EquippedArmor.Name} (Защита: {player.EquippedArmor.Defense})");
            }
            else if (item is UsableItem)
            {
                Console.WriteLine($"Тип: Расходуемый предмет");

                if (item is HealthPotion potion)
                {
                    Console.WriteLine($"Восстановление здоровья: {potion.HealAmount}");
                }
            }

            Console.WriteLine("\nДействия: ");

            if (item is Weapon)
            {
                Console.WriteLine("1. Экипировать");
            }
            else if (item is Armor)
            {
                Console.WriteLine("1. Экипировать");
            }
            else if (item is UsableItem)
            {
                Console.WriteLine("1. Использовать");
            }

            Console.WriteLine("2. Выбросить");
            Console.WriteLine("0. Назад");

            Console.Write("\nВаш выбор: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    if (item is Weapon weapon1)
                    {
                        player.EquipWeapon(weapon1);
                    }
                    else if (item is Armor armor1)
                    {
                        player.EquipArmor(armor1);
                    }
                    else if (item is UsableItem usableItem)
                    {
                        usableItem.Use(player);
                        player.Inventory.Remove(item);
                    }
                    break;
                case "2":
                    player.Inventory.Remove(item);
                    Console.WriteLine($"Предмет {item.Name} выброшен");
                    break;
            }

            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey(true);
        }

        private void EquipItem()
        {
            Console.Clear();
            Console.WriteLine("=== Экипировка ===");

            List<Item> equipableItems = new List<Item>();

            for (int i = 0;  i < player.Inventory.Count; i++)
            {
                if (player.Inventory[i] is Weapon || player.Inventory[i] is Armor)
                {
                    equipableItems.Add(player.Inventory[i]);
                    string type = player.Inventory[i] is Weapon ? "[Оружие]" : "[Броня]";
                    Console.WriteLine($"{equipableItems.Count} - {type} {player.Inventory[i].Name} - {player.Inventory[i].Description}");
                }
            }

            if (equipableItems.Count == 0)
            {
                Console.WriteLine("У вас нет предметов, которые можно использовать");
                Console.WriteLine("Нажмите любую клавишу, чтобы вернуться...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("0. Отмена");
            Console.Write("\nВыберите предмет для экипировки: ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= equipableItems.Count)
            {
                Item item = equipableItems[choice - 1];

                if (item is Weapon weapon)
                {
                    player.EquipWeapon(weapon);
                }
                else if (item is Armor armor)
                {
                    player.EquipArmor(armor);
                }

                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                Console.ReadKey(true);
            }
        }

        private void UseItem()
        {
            Console.Clear();
            Console.WriteLine("=== Использование предмета ===");

            List<UsableItem> usableItems = new List<UsableItem>();

            for (int i = 0;  i < player.Inventory.Count; i++)
            {
                if (player.Inventory[i] is UsableItem)
                {
                    usableItems.Add((UsableItem)player.Inventory[i]);
                    Console.WriteLine($"{usableItems.Count} - {player.Inventory[i].Name} - {player.Inventory[i].Description}");
                }
            }

            if (usableItems.Count == 0)
            {
                Console.WriteLine("У вас нет предметов, которые можно использовать");
                Console.WriteLine("Нажмите любую клавишу, чтобы вернуться...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("0. Отмена");
            Console.Write("\nВыберите предмет для использования: ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= usableItems.Count)
            {
                UsableItem item = usableItems[choice - 1];
                item.Use(player);
                player.Inventory.Remove(item);

                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                Console.ReadKey(true);
            }
        }

        private Item GenerateRandomItem()
        {
            Random random = new Random();
            int itemType = random.Next(3);

            switch (itemType)
            {
                case 0:
                    string[] weaponNames = { "Кинжал", "Короткий меч", "Топор", "Булава", "Лук" };
                    string weaponName = weaponNames[random.Next(weaponNames.Length)];
                    int damage = 3 + random.Next(1, 4) + player.Level;
                    return new Weapon(weaponName, $"Урон: {damage}", damage, damage * 5);

                case 1:
                    string[] armorNames = { "Кожаная куртка", "Кольчуга", "Щит", "Стальной нагрудник", "Шлем" };
                    string armorName = armorNames[random.Next(armorNames.Length)];
                    int defense = 1 + random.Next(1, 3) + player.Level / 2;
                    return new Armor(armorName, $"Защита: {defense}", defense, defense * 8);

                case 2:
                default:
                    int healAmount = 15 + random.Next(5, 16) + player.Level * 3;
                    return new HealthPotion("Зелье здоровья", $"Восстанавливает {healAmount} здоровья", healAmount, healAmount / 2);
            }
        }
    }
}
