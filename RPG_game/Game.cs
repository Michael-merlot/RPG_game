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
            Location village = new Location("Деревня", "Небольшая мирная деревушка. Здесь вы можете отдохнуть и набраться сил.");
            Location forest = new Location("Лес", "Густой темный лес. Говорят, здесь водятся опасные существа.");
            Location cave = new Location("Пещера", "Загадочная пещера в горах. Кто знает, какие сокровище скрыты внутри?");

            world.Add(village);
            world.Add(forest);
            world.Add(cave);

            village.AddNeighbor(forest);
            forest.AddNeighbor(village);
            forest.AddNeighbor(cave);
            cave.AddNeighbor(forest);

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
            Console.Clear();
            Console.WriteLine("=== Инвентарь ===");
            Console.WriteLine($"Золото: {player.Gold}");

            if (player.Inventory.Count == 0)
            {
                Console.WriteLine("Инвентарь пуст");
            }
            else if (player.Inventory.Count >= 1)
            {
                for (int i = 0; i < player.Inventory.Count; i++)
                {
                    Console.WriteLine($"{i + 1} - {player.Inventory[i].Name}");
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
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
