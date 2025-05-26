using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public class Player
    {
        public string Name {  get; private set; }
        public int  Health { get; set; }
        public int MaxHealth { get; private set; }
        public int Level { get; private set; }
        public int Experience { get; private set; }
        public int Gold { get; set; }

        public int Strength { get; private set; } // на урон
        public int Dexterity { get; private set; } // на уклонение
        public int Constitution { get; private set; } // на здоровье 

        public Weapon EquippedWeapon { get; private set; }
        public Armor EquippedArmor { get; private set; }
        
        public List<Item> Inventory { get; set; }

        public Player(string name, int health, int level)
        {
            Name = name;
            Level = level;

            Experience = 0;
            Gold = 10;

            Strength = 5;
            Dexterity = 5;
            Constitution = 5;

            MaxHealth = health + Constitution * 5;
            Health = MaxHealth;

            EquippedWeapon = new Weapon("Старый меч", "Ржавый, но всё еще острый", 5, 0);
            EquippedArmor = new Armor("Потрепанная одежда", "Не обеспечивает особой защиты",1 ,0);

            Inventory = new List<Item>();

            Inventory.Add(new HealthPotion("Малое зелье здоровья", "Восстанавливает 20 здоровья", 20, 5));
        }

        public void AddExperience(int amount)
        {
            Experience += amount;
            Console.WriteLine($"Получено {amount} опыта!");

            int expNeeded = Level * 100;

            if (Experience >= expNeeded)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Level++;
            Experience -= (Level - 1) * 100;

            Console.WriteLine("\n=== Новый уровень! ===");
            Console.WriteLine($"Вы достигнули уровня {Level}");

            Console.WriteLine("\nВыберите характеристику для увеличения: ");
            Console.WriteLine($"1. Сила ({Strength}) - увеличивает урон");
            Console.WriteLine($"2. Ловкость ({Dexterity}) - увеличивает шанс уклонения");
            Console.WriteLine($"3. Телосложение ({Constitution}) - увеличивает здоровье");

            bool validChoice = false;
            while ( !validChoice )
            {
                Console.Write("\nВаш выбор: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Strength += 2;
                        Console.WriteLine($"Сила увеличена до {Strength}");
                        validChoice = true; break;

                    case "2":
                        Dexterity += 2;
                        Console.WriteLine($"Ловкость увеличена до {Dexterity}");
                        validChoice = true; break;

                    case "3":
                        Constitution += 2;
                        Console.WriteLine($"Телосложение увеличено до {Constitution}");
                        validChoice = true; break;

                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова");
                        break;
                }
            }

            Health = MaxHealth;
            Console.WriteLine($"Здоровье полностью восстановлено: {Health}/{MaxHealth}");

            Console.WriteLine("\nНажмите на любую клавишу для продолжения...");
            Console.ReadKey(true);
        }

        private void UpdateMaxHealth()
        {
            int oldMaxHealth = MaxHealth;
            MaxHealth = 100 + Constitution * 5;

            if (oldMaxHealth > 0)
            {
                Health = (Health * MaxHealth) / oldMaxHealth;
            }

            Console.WriteLine($"Максимальное здоровье уввеличено до {MaxHealth}!");
        }

        public int GetAttackDamage()
        {
            return 3 + Strength / 2 + (EquippedWeapon?.Damage ?? 0);
        }

        public int GetDefense()
        {
            return 1 + Dexterity / 4 + (EquippedArmor?.Defense ?? 0);
        }

        public void AddItem(Item item)
        {
            Inventory.Add(item);
            Console.WriteLine($"Получен предмет: {item.Name}");
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (EquippedWeapon != null)
            {
                Inventory.Add(weapon);
            }

            EquippedWeapon = weapon;
            Inventory.Remove(weapon);

            Console.WriteLine($"Экипировано оружие: {weapon.Name}");
        }

        public void EquipArmor(Armor armor)
        {
            if (EquippedArmor != null)
            {
                Inventory.Add(EquippedArmor);
            }

            EquippedArmor = armor;
            Inventory.Remove(armor);

            Console.WriteLine($"Экипирована броня: {armor.Name}");
        }
    }
}
