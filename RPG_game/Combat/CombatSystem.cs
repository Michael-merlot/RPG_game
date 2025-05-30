using RPG_game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public class CombatSystem
    {
        private Player player;
        private Enemy enemy;
        private Random random;
        private bool isCombatActive;
        private bool playerDefensing = false;
        AchievementManager achievementManager;

        public CombatSystem(AchievementManager achievementManager = null)
        {
            random = new Random();
            this.achievementManager = achievementManager;
        }

        public bool StartCombat(Player player, Enemy enemy)
        {
            this.player = player;
            this.enemy = enemy;
            isCombatActive = true;

            Console.Clear();
            Console.WriteLine($"=== Бой ===");
            Console.WriteLine($"=== {player.Name} VS {enemy.Name} ===");
            Console.WriteLine($"{enemy.Description}");

            Boss boss = enemy as Boss;
            if ( boss != null )
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n!!! Битва с Боссом !!!");
                Console.WriteLine(boss.IntroText);
                Console.ResetColor();

                AudioManager.Instance.PlayMusic("Битва");
            }

            Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey();

            while (isCombatActive)
            {
                if (!PlayerTurn()) { break; }

                if (enemy.Health <= 0)
                {
                    if (boss != null)
                    {
                        boss.Defeat();
                    }

                    EndCombat(true);
                    return true;
                }

                if (boss != null && boss.ShouldChangePhase())
                {
                    boss.NextPhase();
                }

                EnemyTurn();

                if (player.Health <= 0)
                {
                    EndCombat(false);
                    return false;
                }
            }

            return player.Health > 0;
        }

        private bool PlayerTurn()
        {
            bool turnCompleted = false;

            while (!turnCompleted)
            {
                Console.Clear();
                DisplayCombatStatus();

                Console.WriteLine("\nВаш ход. Выберите действие: ");
                Console.WriteLine("1. Атаковать");
                Console.WriteLine("2. Защититься");
                Console.WriteLine("3. Использовать предмет");
                Console.WriteLine("4. Попытаться сбежать");

                Console.Write("\nВаш выбор: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PlayerAttack();
                        playerDefensing = false;
                        turnCompleted = true;
                        break;

                    case "2":
                        PlayerDefend();
                        turnCompleted=true;
                        break;

                    case "3":
                        if (UseItem())
                        {
                            playerDefensing = false;
                            turnCompleted = true;
                        }
                        break;

                    case "4":
                        if (TryEscape())
                        {
                            return false;
                        }
                        else
                        {
                            playerDefensing = false;
                            turnCompleted = true;
                        }
                        break;

                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                        Console.ReadKey(true);
                        break;
                }
            }

            return true;

        }

        private void PlayerDefend()
        {
            playerDefensing = true;
            Console.WriteLine("Вы приняли защитную стойку, уменьшив урон до следующего хода.");
            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey(true);
        }

        private void PlayerAttack()
        {
            int damageBase = player.GetAttackDamage();

            int damage = random.Next((int)(damageBase * 0.8), (int)(damageBase * 1.2) + 1);

            bool isCritical = random.Next(100) < 10;

            if (isCritical)
            {
                damage = (int)(damage * 1.5);
                Console.WriteLine("Критический удар");
            }

            int actualDamage = Math.Max(1, damage - enemy.Defense);
            enemy.Health -= actualDamage;

            Console.WriteLine($"Вы атакуете {enemy.Name} и наносите {actualDamage} урона!");
            Console.WriteLine($"Нажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private bool UseItem()
        {
            Console.Clear();
            Console.WriteLine("=== Инвентарь ===");

            List<Item> usableItems = player.Inventory.FindAll(item => item is UsableItem);

            if (usableItems.Count == 0)
            {
                Console.WriteLine("У вас нет предметов, которые можно использовать в бою");
                Console.WriteLine("Нажмите любую клавишу, чтобы вернуться...");
                Console.ReadKey(true);
                return false;
            }

            for (int i = 0; i < usableItems.Count; i++)
            {
                Console.WriteLine($"{i + 1} - {usableItems[i].Name}");
            }

            Console.WriteLine("0. Вернуться");

            Console.Write("\nВыберите предмет: ");

            if (int.TryParse(Console.ReadLine(), out int itemIndex) && itemIndex > 0 && itemIndex <= usableItems.Count)
            {
                UsableItem item = (UsableItem)usableItems[itemIndex - 1];
                item.Use(player);
                player.Inventory.Remove(item);
                return true;
            }

            return false;
        }

        private bool TryEscape()
        {
            int escapeChance = 40 + player.Level * 5 - enemy.Level * 5;
            escapeChance = Math.Clamp(escapeChance, 10, 80);

            if (random.Next(100) < escapeChance)
            {
                Console.WriteLine("Вам удалось сбежать от противника!");
                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                Console.ReadKey(true);
                isCombatActive = false;
                return true;
            }
            else
            {
                Console.WriteLine("Вам не удалось сбежать");
                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить...");
                Console.ReadKey(true);
                return false;
            }
        }

        private void EnemyTurn()
        {
            Console.Clear();
            DisplayCombatStatus();
            Console.WriteLine($"\nХод противника: {enemy.Name}");

            Boss boss = enemy as Boss;

            if (boss != null)
            {
                if (random.Next(100) < boss.SpecialAttackChance)
                {
                    int damage = boss.PerformSpecialAttack();
                    int actualDamage = Math.Max(1, damage - player.GetDefense());
                    if (playerDefensing)
                    {
                        actualDamage = Math.Max(1, actualDamage / 2);
                        Console.WriteLine($"Ваша защитная стойка снижает получаемый урон!");
                    }

                    player.Health -= actualDamage;
                    Console.WriteLine($"Вы получаете {actualDamage} урона от особой атаки босса");
                }
                else
                {
                    int damage = enemy.GetAttackDamage();
                    int actualDamage = Math.Max(1, damage - player.GetDefense());
                    if (playerDefensing)
                    {
                        actualDamage = Math.Max(1, actualDamage / 2);
                        Console.WriteLine($"Ваша защитная стойка снижает получаемый урон!");
                    }

                    player.Health -= actualDamage;
                    Console.WriteLine($"{enemy.Name} атакует вас и наносит {actualDamage} урона!");
                }
            }
            else
            {
                if (random.Next(100) < 80)
                {
                    int damage = enemy.GetAttackDamage();

                    damage = random.Next((int)(damage * 0.8), (int)(damage * 1.2) + 1);

                    int actualDamage = Math.Max(1, damage - player.GetDefense());

                    if (playerDefensing)
                    {
                        actualDamage = Math.Max(1, actualDamage / 2);
                        Console.WriteLine("Ваша защитная стойка снижает получаемый урон!");
                    }

                    player.Health -= actualDamage;

                    Console.WriteLine($"{enemy.Name} атакует вас и наносит {actualDamage} урона!");
                }
                else
                {
                    enemy.DefenseBonus = random.Next(1, 4);
                    Console.WriteLine($"{enemy.Name} принимает защитную стойку (+{enemy.DefenseBonus} к защите)");
                }
            }

            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey(true);

            enemy.DefenseBonus = 0;
        }

        private void DisplayCombatStatus()
        {
            Console.WriteLine($"=== Бой: {player.Name} VS {enemy.Name} ===\n");

            Console.WriteLine($"{player.Name}:");
            DisplayHealthBar(player.Health, player.MaxHealth, ConsoleColor.Green);
            Console.WriteLine($"Здоровье: {player.Health}/{player.MaxHealth} | Атака: {player.GetAttackDamage()} | Защита: {player.GetDefense()}");

            if (playerDefensing)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Статус: защитная стойка (получаемый урон снижен на 50%)");
                Console.ResetColor();
            }
            Console.WriteLine();

            Console.WriteLine($"{enemy.Name}:");
            DisplayHealthBar(enemy.Health, enemy.MaxHealth, ConsoleColor.Red);
            Console.WriteLine($"Здоровье: {enemy.Health}/{enemy.MaxHealth} | Атака: {enemy.GetAttackDamage()} | Защита: {enemy.Defense + enemy.DefenseBonus}");

            if (enemy.DefenseBonus > 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Статус: Защитная стойка");
                Console.ResetColor();
            }
            Console.WriteLine();
        }

        private void DisplayHealthBar(int health, int maxHealth, ConsoleColor color)
        {
            int barLength = 20;
            int filledLength = (int)Math.Ceiling((double)health / maxHealth * barLength);

            Console.Write("[");
            ConsoleColor originalcolor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            for (int i = 0; i < barLength; i++)
            {
                if (i < filledLength)
                {
                    Console.Write("|||");
                }
                else
                {
                    Console.Write(" ");
                }
            }

            Console.ForegroundColor = originalcolor;
            Console.WriteLine("]");
        }

        private void EndCombat(bool playerWon)
        {
            Console.Clear();

            if (playerWon)
            {
                Boss boss = enemy as Boss;

                if (boss != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"=== Великая победа над боссом! ===");
                    Console.WriteLine(boss.DefeatText);
                    Console.ResetColor();

                    AudioManager.Instance.PlaySoundEffect("Победа");

                    int expReward = boss.ExpReward;
                    int goldReward = boss.GoldReward;
                    Console.WriteLine($"Получено опыта: {expReward}");
                    Console.WriteLine($"Получено золота: {goldReward}");

                    if (boss.GuaranteedLoot.Count > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"\n=== Особоая награда от босса!");
                        Console.ResetColor();

                        foreach (Item item in boss.GuaranteedLoot)
                        {
                            player.AddItem(item);
                            Console.WriteLine($"Получен предмет: {item.Name} - {item.Description}");
                        }
                    }

                    if (achievementManager != null)
                    {
                        achievementManager.UpdateAchievement("kill_boss", 1);

                        if (CheckAllBossesDefeated())
                        {
                            achievementManager.UpdateAchievement("kill_all_bosses", 1);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("=== Победа ===");
                    Console.WriteLine($"Вы победили: {enemy.Name}");

                    int expReward = enemy.ExpReward;
                    int goldReward = enemy.GoldReward;

                    player.AddExperience(expReward);
                    player.Gold += goldReward;

                    Console.WriteLine($"Получено опыта: {expReward}");
                    Console.WriteLine($"Получено золота: {goldReward}");

                    if (random.Next(100) < 30)
                    {
                        Item loot = GenerateLoot();
                        player.AddItem(loot);
                        Console.WriteLine($"Найден предмет: {loot.Name}");
                    }
                }
            }
            else
            {
                Console.WriteLine("=== Поражение ===");
                Console.WriteLine($"Вы пали в бою с {enemy.Name}");

                AudioManager.Instance.PlaySoundEffect("Поражение");
            }

            Console.WriteLine($"\nНажмите любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private bool CheckAllBossesDefeated()
        {
            return false;
        }

        private Item GenerateLoot()
        {
            string[] itemTypes = { "Оружие", "Броня", "Зелье" };
            string type = itemTypes[random.Next(itemTypes.Length)];

            switch (type)
            {
                case "Оружие":
                    string[] weapons = { "Короткий меч", "Кинжал", "Дубина", "Лук" };
                    string weaponName = weapons[random.Next(weapons.Length)];
                    int damage = 5 + random.Next(1, 6) + player.Level;
                    return new Weapon(weaponName, $"Урон: {damage}", damage, 10 * damage);
                case "Броня":
                    string[] armors = { "Кожаная броня", "Кольчуга", "Щит", "Шлем" };
                    string armorName = armors[random.Next(armors.Length)];
                    int defense = 2 + random.Next(1, 4) + player.Level / 2;
                    return new Armor(armorName, $"Защита: {defense}", defense, 15 * defense);
                case "Зелье":
                default:
                    int healing = 20 + random.Next(10, 21) + player.Level * 5;
                    return new HealthPotion("Зелье лечения", $"Восстанавливает {healing} здоровья", healing, 10 + healing / 2);
            }
        }
    }
}
