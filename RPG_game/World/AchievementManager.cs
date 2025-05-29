using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public class AchievementManager
    {
        private List<Achievement> achievements;
        private Player player;

        public AchievementManager(Player player)
        {
            this.player = player;
            achievements = new List<Achievement>();
            InitializeAchievements();
        }

        private void InitializeAchievements()
        {
            achievements.Add(new Achievement("explore_all", "Исследователь мира", "Посетите все локации", AchievementType.Exploration, 4, 50, 100));
            achievements.Add(new Achievement("explore_dungeon", "Искатель приключений", "Исследуйте подземелье", AchievementType.Exploration, 1, 20, 30));

            achievements.Add(new Achievement("kill_enemies", "Боевое крещение", "Победите 10 врагов", AchievementType.Combat, 10, 30, 50));
            achievements.Add(new Achievement("kill_boss", "Истребитель демонов", "Победите любого босса", AchievementType.Combat, 1, 100, 150));
            achievements.Add(new Achievement("kill_all_bosses", "Легенда", "Победите всех боссов", AchievementType.Combat, 3, 300, 500));

            achievements.Add(new Achievement("collect_gold", "Охотник за сокровищами", "Соберите 500 золота", AchievementType.Collection, 500, 50, 50));
            achievements.Add(new Achievement("collect_items", "Коллекционер", "Соберите 15 предметов", AchievementType.Collection, 15, 30, 40));

            achievements.Add(new Achievement("complete_quest", "Помощник", "Выполните первое задание", AchievementType.Quest, 1, 20, 30));
            achievements.Add(new Achievement("complete_quests", "Герой города", "Выполните 5 заданий", AchievementType.Quest, 5, 100, 150));

            achievements.Add(new Achievement("reach_level", "Наставник", "Достигните 10 уровня", AchievementType.Special, 10, 100, 200));
        }
        public void UpdateAchievement(string achievementId, int amount = 1)
        {
            foreach (Achievement achievement in achievements)
            {
                if (achievement.Id == achievementId)
                {
                    achievement.UpdateProgress(amount);
                    if (achievement.IsUnlocked)
                    {
                        achievement.GiveReward(player);
                    }
                    break;
                }
            }
        }
        public void DisplayAchievements()
        {
            Console.Clear();
            Console.WriteLine("=== Достижения ===\n");

            DisplayAchievementsByType(AchievementType.Exploration, "Исследования");
            DisplayAchievementsByType(AchievementType.Combat, "Сражения");
            DisplayAchievementsByType(AchievementType.Collection, "Коллекция");
            DisplayAchievementsByType(AchievementType.Quest, "Квесты");
            DisplayAchievementsByType(AchievementType.Special, "Особое");

            Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey();
        }
        private void DisplayAchievementsByType(AchievementType type, string categoryName)
        {
            Console.WriteLine($"=== {categoryName} ===");

            bool hasAchievements = false;
            foreach (Achievement achievement in achievements)
            {
                if (achievement.Type == type)
                {
                    hasAchievements = true;
                    string status = achievement.IsUnlocked ? "[V]" : "[]";
                    Console.Write($"{status} - {achievement.Name} - {achievement.Description}");

                    if (achievement.IsUnlocked)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }

                    Console.WriteLine(achievement.GetProgressString());
                    Console.ResetColor();
                }
            }

            if (!hasAchievements)
            {
                Console.WriteLine("Нет доступных достижений");
            }
            Console.WriteLine();
        }
        public void CheckAchievements()
        {
            UpdateAchievement("reach_level", player.Level);
            UpdateAchievement("collect_gold", player.Gold);
            UpdateAchievement("collect_items", player.Inventory.Count);
        }
        public int GetUnlockedCount()
        {
            int count = 0;
            foreach (Achievement achievement in achievements)
            {
                if (achievement.IsUnlocked)
                {
                    count++;
                }
            }
            return count;
        }
        public int GetTotalCount()
        {
            return achievements.Count;
        }
    }
}
