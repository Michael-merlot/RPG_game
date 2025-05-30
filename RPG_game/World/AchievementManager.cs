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
        private bool isCheckingAchievement = false;

        public AchievementManager(Player player)
        {
            this.player = player;
            achievements = new List<Achievement>();
            InitializeAchievements();
        }

        private void InitializeAchievements()
        {
            achievements.Add(new Achievement("explore_all", "Исследователь мира", "Посетите все локации. ", AchievementType.Exploration, 4, 50, 100));
            achievements.Add(new Achievement("explore_dungeon", "Искатель приключений", "Исследуйте подземелье. ", AchievementType.Exploration, 1, 20, 30));

            achievements.Add(new Achievement("kill_enemies", "Боевое крещение", "Победите 10 врагов. ", AchievementType.Combat, 10, 30, 50));
            achievements.Add(new Achievement("kill_boss", "Истребитель демонов", "Победите любого босса. ", AchievementType.Combat, 1, 100, 150));
            achievements.Add(new Achievement("kill_all_bosses", "Легенда", "Победите всех боссов. ", AchievementType.Combat, 3, 300, 500));

            achievements.Add(new Achievement("collect_gold", "Охотник за сокровищами", "Соберите 500 золота. ", AchievementType.Collection, 500, 50, 50, onlyCheckOnce: true));
            achievements.Add(new Achievement("collect_items", "Коллекционер", "Соберите 15 предметов. ", AchievementType.Collection, 15, 30, 40));

            achievements.Add(new Achievement("complete_quest", "Помощник", "Выполните первое задание. ", AchievementType.Quest, 1, 20, 30));
            achievements.Add(new Achievement("complete_quests", "Герой города", "Выполните 5 заданий. ", AchievementType.Quest, 5, 100, 150));

            achievements.Add(new Achievement("reach_level", "Первые шаги", "Достигните 2 уровня. ", AchievementType.Special, 2, 50, 100));
            achievements.Add(new Achievement("master_level", "Наставник", "Достигните 10 уровня. ", AchievementType.Special, 10, 100, 200));
        }
        public void UpdateAchievement(string achievementId, int amount = 1)
        {
            foreach (Achievement achievement in achievements)
            {
                if (achievement.Id == achievementId && !achievement.IsUnlocked)
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
            foreach (Achievement achievement in achievements)
            {
                if (!achievement.IsUnlocked)
                {
                    switch (achievement.Id)
                    {
                        case "reach_level":
                            if (player.Level >= 2)
                            {
                                achievement.Unlock();
                                achievement.GiveReward(player);
                            }
                            break;

                        case "master_level":
                            if (player.Level >= 10)
                            {
                                achievement.Unlock();
                                achievement.GiveReward(player);
                            }
                            break;

                        case "collect_gold":
                            if (player.Gold >= 500)
                            {
                                achievement.Unlock();
                                achievement.GiveReward(player);
                            }
                            break;
                            
                    }
                }
            }
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
