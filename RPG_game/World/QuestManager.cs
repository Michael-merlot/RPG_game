using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public class QuestManager
    {
        private List<Quest> availableQuest;
        private List<Quest> activeQuest;
        private List<Quest> completedQuest;
        private Player player;
        
        public QuestManager(Player player)
        {
            this.player = player;
            availableQuest = new List<Quest>();
            activeQuest = new List<Quest>();
            completedQuest = new List<Quest>();
        }
        public void AddQuest(Quest quest)
        {
            availableQuest.Add(quest);
        }
        public void StartQuest(Quest quest)
        {
            if (availableQuest.Contains(quest))
            {
                quest.Start();
                availableQuest.Remove(quest);
                activeQuest.Add(quest);
            }
        }
        public void UpdateQuestProgress(QuestType type, string targetName, int amount = 1)
        {
            foreach (Quest quest in activeQuest)
            {
                if (quest.Type == type)
                {
                    quest.UpdateProgress(targetName, amount);
                }
            }
        }
        public void CompleteQuest(Quest quest)
        {
            if (activeQuest.Contains(quest) && quest.IsReadyToComplete())
            {
                quest.Complete(player);
                activeQuest.Remove(quest);
                completedQuest.Add(quest);
            }
        }
        public Quest GetQuestById(string id)
        {
            foreach (Quest quest in availableQuest)
            {
                if (quest.Id == id) {  return quest; }
            }
            foreach (Quest quest in activeQuest)
            {
                if (quest.Id == id) { return quest; }
            }
            foreach (Quest quest in completedQuest)
            {
                if (quest.Id == id) { return quest; }
            }

            return null;
        }
        public void DisplayActiveQuests()
        {
            Console.Clear();
            Console.WriteLine($"=== Активные квесты ===\n");

            if (activeQuest.Count == 0)
            {
                Console.WriteLine($"У вас нет активных квестов");
            }
            else
            {
                for (int i = 0; i < activeQuest.Count; i++)
                {
                    Quest quest = activeQuest[i];
                    Console.WriteLine($"{i + 1} - {quest.Name}");
                    Console.WriteLine($"    {quest.Description}");
                    Console.WriteLine($"    Прогресс: {quest.GetProgressString()}");
                    Console.WriteLine($"    Награды: {quest.GoldReward} золота, {quest.ExpReward} опыта");

                    if (quest.ItemRewards.Count > 0)
                    {
                        Console.Write($"    Предметы: ");
                        for (int j = 0; j < quest.ItemRewards.Count; j++)
                        {
                            Console.WriteLine(quest.ItemRewards[j].Name);
                            if (j < quest.ItemRewards.Count - 1)
                            {
                                Console.WriteLine(", ");
                            }
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Нажмите любую клавишу, чтобы вернуться...");
            Console.ReadKey(true);
        }

        public int GetActiveQuestCount()
        {
            return activeQuest.Count;
        }
    }
}
