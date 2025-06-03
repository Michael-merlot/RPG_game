using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public enum AchievementType
    {
        Exploration,
        Combat,
        Collection,
        Quest,
        Special
    }
    public class Achievement
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public AchievementType Type { get; private set; }
        public bool IsUnlocked { get; private set; }
        public int ProgressCurrent { get; private set; }
        public int ProgressRequired { get; private set;}
        public int RewardGold { get; private set; }
        public int RewardExp { get; private set; }

        private DateTime unlockTime;
        private bool wasChecked = false;
        private bool onlyCheckOnce;
        public bool RewardGiven { get; private set; } = false;

        public Achievement(string id, string name, string description, AchievementType type, int progressRequired, int rewardGold, int rewardExp, bool onlyCheckOnce = false)
        {
            Id = id;
            Name = name;
            Description = description;
            Type = type;
            IsUnlocked = false;
            ProgressCurrent = 0;
            ProgressRequired = progressRequired;
            RewardGold = rewardGold;
            RewardExp = rewardExp;
            this.onlyCheckOnce = onlyCheckOnce; 
        }
        public Achievement(string id, string name, string description, AchievementType type, int rewardGold, int rewardExp) : this (id, name, description, type, 1, rewardGold, rewardExp)
        {

        }
        public void UpdateProgress(int amount)
        {
            if (IsUnlocked || (onlyCheckOnce && wasChecked)) { return; }

            wasChecked = onlyCheckOnce;
            ProgressCurrent += amount;

            if (ProgressCurrent >= ProgressRequired)
            {
                Unlock();
            }
        }
        public void Unlock()
        {
            if (IsUnlocked) { return; }

            IsUnlocked=true;
            ProgressCurrent = ProgressRequired;
            unlockTime = DateTime.Now;
            AudioManager.Instance.PlayMusic("Победа");
            DisplayUnlockNotification();
        }
        private void DisplayUnlockNotification()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n╔════════════════════════════════════════╗");
            Console.WriteLine($"║  Достижение разблокировано!            ║");
            Console.WriteLine($"║  {Name.PadRight(35)} ║");
            Console.WriteLine($"║  {Description.PadRight(35)} ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.ResetColor();
        }
        public void GiveReward(Player player)
        {
            if (!IsUnlocked || RewardGiven) return;

            RewardGiven = true;

            player.Gold += RewardGold;
            player.AddExperience(RewardExp);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nПолучена награда за достижение \"{Name}\":");
            Console.WriteLine($"+ {RewardGold} золота");
            Console.WriteLine($"+ {RewardExp} опыта");
            Console.ResetColor();
        }

        public string GetProgressString()
        {
            if (IsUnlocked)
            {
                return $"Разблокировано {unlockTime.ToString("dd.MM.yyyy HH:mm")}";
            }
            else
            {
                return $"Прогресс: {ProgressCurrent}/{ProgressRequired}";
            }
        }
    }
}
