using NAudio.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security;
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
        private QuestManager questManager;
        private AchievementManager achievementManager;
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

            forest.SetBoss(BossFactory.CreateForestGiant(1));
            cave.SetBoss(BossFactory.CreateSpiderQueen(2));
            dungeon.SetBoss(BossFactory.CreateAncientGuardin(3));

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
            village.AddNPC(new Trader("Лесник", "Опытный охотник и следопыт, хорошо знающий лес."));

            currentLocation = village;
        }

        public void Start()
        {

            AudioManager.Instance.PlayMusic("Меню");
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

            achievementManager = new AchievementManager(player);
            questManager = new QuestManager(player);
            InitializeQuests();

            Console.WriteLine($"Здравствуй, {name}! Похоже... ты здесь впервые, я прав? " +
                $"\nСмотри, до тебя бывали такие же ребята, как и ты, но всех ждало только одно - смерть"
                + $"\nПоэтому будь осторожен, может, тебе удастся дойти до... пх... пх... *связь прервана*");

            // Thread.Sleep(8000);

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

                bool isViewingCommand = command == "журнал заданий" || command == "достижения" || command == "настройки";
                ProcessCommand(command);

                if (!isViewingCommand)
                {
                    achievementManager.CheckAchievements();
                }

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

            AudioManager.Instance.PlayMusic(currentLocation.Name);

            Console.WriteLine("\nДоступные пути: ");
            foreach (Location neighbor in currentLocation.Neighbors)
            {
                Console.WriteLine($"- {neighbor.Name}");
            }

            if (currentLocation.NPCs.Count > 0)
            {
                Console.WriteLine("\nЖители: ");

                for (int i = 0; i < currentLocation.NPCs.Count; i++)
                {
                    Console.WriteLine($"- {currentLocation.NPCs[i].Name}");
                }
            }

            if (currentLocation.Boss != null)
            {
                if (currentLocation.BossDefeated)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"\n[Босс {currentLocation.Boss.Title} {currentLocation.Boss.Name} побежден] ");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[ВНИМАНИЕ: В этой локации обитает могущественный босс: {currentLocation.Boss.Title} {currentLocation.Boss.Name}] ");
                    Console.ResetColor();
                }
            }

            Console.WriteLine("\n============================================");
            Console.WriteLine($"{player.Name} | Уровень: {player.Level} | Здоровье: {player.Health}/{player.MaxHealth} | Золото: {player.Gold}");   
            if (questManager.GetActiveQuestCount() > 0)
            {
                Console.WriteLine($"Активные квесты: {questManager.GetActiveQuestCount()}");
            }

            Console.WriteLine("\nДействия: ");
            Console.WriteLine("1. [путешествовать] - перейти в другую локацию");
            Console.WriteLine("2. [инвентарь] - открыть инвентарь");
            Console.WriteLine("3. [Журнал заданий] - просмотреть активные квесты");
            Console.WriteLine("4. [Достижения] - просмотреть прогресс достижений");

            int optionNumber = 6;

            if (currentLocation.NPCs.Count > 0)
            {
                Console.WriteLine($"{optionNumber}. [общаться] - поговорить с жителями");
                optionNumber++;
            }
            if (!currentLocation.IsSafeZone)
            {
                Console.WriteLine($"{optionNumber}. [исследовать] - искать приключений");
                optionNumber++;
            }
            if (!currentLocation.IsSafeZone && currentLocation.HasUnderfeatedBoss())
            {
                Console.WriteLine($"{optionNumber} [бросить вызов боссу] - сразиться с хозяином этих земель");
                optionNumber++;
            }

            Console.WriteLine("0. [выход] - выйти из игры");

        }

        public string GetUserInput()
        {
            Console.Write("\nВаше действие (введите номер): ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    return "путешествовать";

                case "2":
                    return "инвентарь";

                case "3":
                    return "журнал заданий";

                case "4":
                    return "достижения";

                case "5":
                    if (currentLocation.NPCs.Count > 0)
                    {
                        return "общаться";
                    }
                    break;

                case "6":
                    if (!currentLocation.IsSafeZone)
                    {
                        return "исследовать";
                    }
                    break;

                case "7":
                    if (!currentLocation.IsSafeZone && currentLocation.HasUnderfeatedBoss())
                    {
                        return "бросить вызов боссу";
                    }
                    break;

                case "0":
                    return "выход";
            }

            return input;
        }

        private void ProcessCommand(string command)
        {
            switch (command)
            {
                case "исследовать":
                    if (!currentLocation.IsSafeZone)
                    {
                        Explore();
                    }
                    else
                    {
                        Console.WriteLine("Это безопасная зона. Здесь нечего исследовать.");
                        Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                        Console.ReadKey(true);
                    }
                    break;

                case "путешествовать":
                    Travel();
                    break;

                case "инвентарь":
                    ShowInventory();
                    break;

                case "журнал заданий":
                case "квесты":
                    questManager.DisplayActiveQuests();
                    break;

                case "достижения":
                    achievementManager.DisplayAchievements();
                    break;

                case "общаться":
                    if (currentLocation.NPCs.Count > 0)
                    {
                        TalkToNPC();
                    }
                    else
                    {
                        Console.WriteLine("Здесь никого нет");
                        Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                        Console.ReadKey(true);
                    }
                    break;

                case "бросить вызов боссу":
                    if (!currentLocation.IsSafeZone && currentLocation.HasUnderfeatedBoss())
                    {
                        FightBoss();
                    }
                    else
                    {
                        Console.WriteLine("В этой локации нет босса или он уже побежден.");
                        Console.WriteLine("Нажмите любую клавишу...");
                        Console.ReadKey(true);
                    }
                    break;

                case "настройки":
                    ShowSettings();
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
        private void FightBoss()
        {
            Console.Clear();
            Console.WriteLine("Вы решили бросить вызов хозяину этих земель");

            Boss boss = currentLocation.Boss;
            if (boss == null && currentLocation.BossDefeated)
            {
                Console.WriteLine("В этой локации нет босса или он уже побежден.");
                Console.WriteLine("Нажмите любую клавишу...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("\nПосле долгих поисков вы находите логово босса.");
            Console.WriteLine($"\nПеред вами предстает {boss.Title} {boss.Name}");
            Console.WriteLine(boss.Description);

            Console.WriteLine("\nГотовы ли вы сразиться с этим могущественным противником?");
            Console.WriteLine("1. Да, я готов к битве");
            Console.WriteLine("2. Нет, я вернусь позже");

            Console.Write("\n");
        }
        private void TalkToNPC()
        {
            Console.Clear();
            Console.WriteLine($"=== Жители {currentLocation.Name} ===");

            for (int i = 0; i < currentLocation.NPCs.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {currentLocation.NPCs[i].Name} - {currentLocation.NPCs[i].Description}");
            }
            Console.WriteLine("0. Вернуться");

            Console.Write("\nС кем хотите поговорить (введите номер): ");

            if (int.TryParse(Console.ReadLine(), out int numberIndex) &&  numberIndex > 0 && numberIndex <= currentLocation.NPCs.Count)
            {
                currentLocation.NPCs[numberIndex - 1].Interact(player, this);
            }
        }

        private void Explore()
        {
            if (currentLocation.IsSafeZone)
            {
                Console.WriteLine("Это безопасная зона. Здесь нечего исследовать");
                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                Console.ReadKey(true);
                return;
            }

            Console.Clear();
            Console.WriteLine($"Вы исследуете: {currentLocation.Name}");

            Random random = new Random();
            int eventType;

            if (currentLocation.Name.ToLower() == "подземелье")
            {
                eventType = random.Next(60);

                if (eventType < 60)
                {
                    Enemy enemy = EnemyFactory.CreateRandomEnemy(currentLocation.Name, player.Level);
                    Console.WriteLine($"Вы столкнулись с врагом: {enemy.Name}");
                    Console.WriteLine("Нажмите любую клавишу, чтобы начать бой...");
                    Console.ReadKey(true);

                    CombatSystem combatSystem = new CombatSystem();
                    bool playerWon = combatSystem.StartCombat(player, enemy);

                    if (!playerWon)
                    {
                        isRunning = false;
                        return;
                    }
                }
                else if (eventType < 80)
                {
                    Item foundItem = GenerateRandomItem();
                    player.AddItem(foundItem);
                }
                else
                {
                    int gold = random.Next(10, 31) + player.Level * 2;
                    player.Gold += gold;
                    Console.WriteLine($"Вы нашли {gold} золота!");
                }
            }
            else
            {
                eventType = random.Next(100);

                if ( eventType < 40)
                {
                    Console.WriteLine($"Вы ничего не нашли интересного");
                }
                else if (eventType < 70)
                {
                    Enemy enemy = EnemyFactory.CreateRandomEnemy(currentLocation.Name, player.Level);
                    Console.WriteLine($"Вы столкнулись с врагом: {enemy.Name}");
                    Console.WriteLine("Нажмите любую клавишу, чтобы начать бой...");
                    Console.ReadKey(true);

                    CombatSystem combatSystem = new CombatSystem();
                    bool playerWon = combatSystem.StartCombat(player, enemy);

                    if (!playerWon)
                    {
                        isRunning = false;
                        return;
                    }

                    if (playerWon)
                    {
                        questManager.UpdateQuestProgress(QuestType.Kill, enemy.Name, 1);
                        achievementManager.UpdateAchievement("kill_enemies", 1);
                    }
                }
                else if (eventType < 85)
                {
                    Item foundItem = GenerateRandomItem();
                    player.AddItem(foundItem);
                }
                else
                {
                    int gold = random.Next(5, 16) + player.Level;
                    player.Gold += gold;
                    Console.WriteLine($"Вы нашли {gold} золота!");
                }
            }

            questManager.UpdateQuestProgress(QuestType.Explore, currentLocation.Name, 1);
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private void Travel()
        {
            Console.Clear();
            Console.WriteLine("=== Путешествие ===");
            Console.WriteLine("Куда вы хотите отправиться?");

            for (int i = 0; i < currentLocation.Neighbors.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {currentLocation.Neighbors[i].Name}");
            }

            Console.WriteLine("0. Вернуться");

            Console.Write("\nВаш выбор (введите номер): ");

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

                    Console.WriteLine($"\nДействия: ");
                    Console.WriteLine("1. Использовать предмет");
                    Console.WriteLine("2. Экипировать предмет");
                    Console.WriteLine("3. Подробнее о предмете");
                    Console.WriteLine("0. Назад");

                    Console.Write("\nВаш выбор (ввведите номер): ");

                    if (!int.TryParse(Console.ReadLine(), out int choice))
                    {
                        choice = 0;
                    }

                    switch (choice)
                    {
                        case 1:
                            UseItem();
                            break;
                        case 2:
                            EquipItem();
                            break;
                        case 3:
                            SelectItemDetails();
                            break;
                        case 0:
                        default:
                            return;
                    }
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private void SelectItemDetails()
        {
            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("Инвентарь пуст");
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey(true);
            }

            Console.Clear();
            Console.WriteLine("=== Выбор предмета ===");

            for (int i = 0;  i < player.Inventory.Count; i++)
            {
                string itemType = player.Inventory[i] is Weapon ? "[Оружие]" : 
                    player.Inventory[i] is Armor ? "[Броня]" :
                    player.Inventory[i] is HealthPotion ? "[Предмет]" : "";

                Console.WriteLine($"{i + 1} - {itemType} {player.Inventory[i].Name}");
            }

            Console.WriteLine("0. Назад");

            Console.Write("\nВыберите предмет (номер): ");

            if (int.TryParse(Console.ReadLine(), out int itemIndex) && itemIndex > 0 && itemIndex <= player.Inventory.Count)
            {
                ShowItemDetails(player.Inventory[itemIndex - 1]);
            }
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

            Console.Write("\nВаш выбор (введите номер): ");
            
            if (int.TryParse(Console.ReadLine(), out int actionChoice))
            {
                switch (actionChoice)
                {
                    case 1:
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
                    case 2:
                        player.Inventory.Remove(item);
                        Console.WriteLine($"Предмет {item.Name} выброшен.");
                        break;
                }
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

        private Item GenerateRandomItem(bool isRare = false)
        {
            Random random = new Random();
            int itemType = random.Next(3);

            switch (itemType)
            {
                case 0:
                    string[] weaponNames = { "Кинжал", "Короткий меч", "Топор", "Булава", "Лук" };
                    string[] rareWeaponNames = { "Эльфийский клинок", "Двуручный меч", "Боевой топор", "Магический посох", "Композитный лук" };

                    string weaponName;
                    int damage;

                    if (isRare && random.Next(100) < 40)
                    {
                        weaponName = rareWeaponNames[random.Next(weaponNames.Length)];
                        damage = 5 + random.Next(3, 7) + player.Level;
                    }
                    else
                    {
                        weaponName = weaponNames[random.Next(weaponNames.Length)];
                        damage = 5 + random.Next(1, 4) + player.Level;
                    }

                    return new Weapon(weaponName, $"Урон: {damage}", damage, damage * 5);

                case 1:
                    string[] armorNames = { "Кожаная куртка", "Кольчуга", "Щит", "Стальной нагрудник", "Шлем" };
                    string[] rareArmorsNames = { "Мифриловая кольчуга", "Эльфийский плащ", "Башенный щит", "Доспех рыцаря", "Шлем стража"};

                    string armorName;
                    int defense;

                    if (isRare && random.Next(100) < 40)
                    {
                        armorName = rareArmorsNames[random.Next(armorNames.Length)];
                        defense = 5 + random.Next(2, 5) + player.Level / 2;
                    }
                    else
                    {
                        armorName = armorNames[random.Next(armorNames.Length)];
                        defense = 5 + random.Next(1, 3) + player.Level / 2;
                    }

                    return new Armor(armorName, $"Защита: {defense}", defense, defense * 8);

                case 2:
                default:
                    int healAmount;
                    string potionName;

                    if (isRare && random.Next(100) < 40)
                    {
                        healAmount = 40 + random.Next(10, 31) + player.Level * 4;
                        potionName = "Большое зелье здоровья";
                    }
                    else
                    {
                        healAmount = 15 + random.Next(5, 16) + player.Level * 2;
                        potionName = "Зелье здоровья";
                    }

                    return new HealthPotion(potionName, $"Восстанавливает {healAmount} здоровья", healAmount, healAmount / 2);
            }
        }

        private void InitializeQuests()
        {
            // квест на убийство волков
            Quest wolfQuest = new Quest("wolf_hunt", "Охота на волков", "Лесник сообщает, что в лесу слишком много волков. Нужно избавиться от нескольких.",
                QuestType.Kill, GetLocationByName("Лес"), GetNPCByName("Лесник"), 30, 50);
            wolfQuest.AddObjective("Убить волка", 3);
            wolfQuest.AddItemReward(new HealthPotion("Большое зелье здоровья", "Восстанавливает 60 здоровья", 60, 30));

            // квест на изучение пещеры
            Quest caveQuest = new Quest("cave_explore", "Тайны пещеры", "Алхимик Маркус интересуется странными кристаллами, которые можно найти в пещере.",
                QuestType.Explore, GetLocationByName("Пещера"), GetNPCByName("Алхимик Маркус"), 40, 70);
            caveQuest.AddObjective("Исследовать глубины пещеры", 1);
            caveQuest.AddItemReward(new Weapon("Зачарованный кинжал", "Магическое оружие алхимика", 8, 45));

            // квест на победу над боссом
            Quest bossQuest = new Quest("dungeon_boss", "Древнее зло", "Кузнец Торим слышал о древнем страже, обитающем в подземелье. Он просит избавиться от этой угрозы.",
    QuestType.Boss, GetLocationByName("Подземелье"), GetNPCByName("Кузнец Торим"), 100, 200);
            bossQuest.AddObjective("Победить Древнего Стража", 1);
            bossQuest.AddItemReward(new Armor("Доспех героя", "Мощная броня, выкованная кузнецом в благодарность", 10, 120));

            questManager.AddQuest(wolfQuest);
            questManager.AddQuest(caveQuest);
            questManager.AddQuest(bossQuest);

            GetNPCByName("Лесник").AddQuest(wolfQuest);
            GetNPCByName("Алхимик Маркус").AddQuest(caveQuest);
            GetNPCByName("Кузнец Торим").AddQuest(bossQuest);
        }

        private NPC GetNPCByName(string name)
        {
            foreach (Location location in world)
            {
                foreach (NPC npc in location.NPCs)
                {
                    if (npc.Name == name) return npc;
                }
            }
            return null;
        }

        private Location GetLocationByName(string name)
        {
            foreach (Location location in world)
            {
                if (location.Name == name) return location;
            }
            return null;
        }

        public bool CheckAllBossesDefeated()
        {
            foreach (Location location in world)
            {
                if (location.HasUnderfeatedBoss())
                {
                    return false;
                }
            }
            return true;
        }

        private void ShowSettings()
        {
            bool exitSettings = false;

            while (!exitSettings)
            {
                Console.Clear();
                Console.WriteLine("=== Настройки игры ===");
                Console.WriteLine($"\n1. Музыка: {(AudioManager.Instance.IsMusicEnabled() ? "Включена" : "Отключена")}");
                Console.WriteLine($"2. Громкость: {(int)(AudioManager.Instance.GetVolume() * 100)}%");
                Console.WriteLine($"3. Информация об аудио");
                Console.WriteLine("0. Назад");

                Console.Write("\nВыберите настройку (номер): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ToggleMusic();
                        break;

                    case "2":
                        AdjustMusic();
                        break;

                    case "3":
                        ShowAudioInfo();
                        break;

                    case "0":
                        exitSettings = true;
                        break;

                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }

            }
        }

        private void ToggleMusic()
        {
            bool currentState = AudioManager.Instance.IsMusicEnabled();
            AudioManager.Instance.SetMusicEnabled(!currentState);

            if (!currentState)
            {
                AudioManager.Instance.PlayMusic(currentLocation.Name);
            }

            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey(true);
        }

        private void AdjustMusic()
        {
            Console.Clear();
            Console.WriteLine("=== Настройка громкости ===");
            Console.WriteLine("\nВыберите уровень громкости: ");
            Console.WriteLine("1. Тихо (25%)");
            Console.WriteLine("2. Средне (50%)");
            Console.WriteLine("3. Громко (75%)");
            Console.WriteLine("4. Максимально (100%)");
            Console.WriteLine("0. Назад");

            Console.Write("\nВаш выбор: ");
            string choice = Console.ReadLine();

            float newVolume = AudioManager.Instance.GetVolume();

            switch (choice)
            {
                case "1":
                    newVolume = 0.25f;
                    break;

                case "2":
                    newVolume = 0.5f;
                    break;

                case "3":
                    newVolume = 0.75f;
                    break;

                case "4":
                    newVolume = 1.0f;
                    break;
            }

            AudioManager.Instance.SetVolume(newVolume);
            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey(true);
        }

        private void ShowAudioInfo()
        {
            Console.Clear();
            AudioManager.Instance.ShowAudioStatus();
            Console.WriteLine("Нажмите любую клавишу, чтобы вернуться...");
            Console.ReadKey(true);
        }

        public void UpdateGame()
        {

        }
    }
}

/*
 * 1) Исправить RPG_Game.Weapon - при покупке предметов у торговцев
 * 2) Реализовать босса в игре и полностью подключить его к релизной версии
 * 3) Подключить музыку к игре и чтобы всё работало
 * 4) Пофиксить баг, когда получаешь уровень (или любое достижение), завершаешь настройку, продолжаешь и на долю секунды выскакивает сообщение о том, что получено 
 * Достижение и снова меню с выбором, где выбираешь [путешествовать] и тп
 */

