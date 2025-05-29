using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public enum QuestType
    {
        Fetch,
        Kill,
        Explore,
        Boss,
        Delivery
    }
    public enum QuestStatus
    {
        NotStarted,
        Active,
        Completed,
        Finished
    }
    public class Quest
    {
        public string Id {  get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public QuestType Type { get; private set; }
        public QuestStatus Status { get; private set;}
        public Location TargetLocation { get; private set; }
        public NPC QuestGiver { get; private set; }

        public Dictionary<string, int> Objectives { get; private set; }
        public Dictionary<string, int> CurrentProgress {  get; private set; }

        public int GoldReward { get; private set; }
        public int ExpReward { get; private set; }
        public List<Item> ItemRewards { get; private set; }

        public Quest(string id, string name, string description, QuestType type, Location targetLocation, NPC questGiver, int goldReward, int expReward)
        {
            Id = id;
            Name = name;
            Description = description;
            Type = type;
            TargetLocation = targetLocation;
            QuestGiver = questGiver;
            GoldReward = goldReward;
            ExpReward = expReward;

            Objectives = new Dictionary<string, int>();
            CurrentProgress = new Dictionary<string, int>();
            ItemRewards = new List<Item>();
        }
        public void AddObjective(string objectiveName, int requiredAmount)
        {
            Objectives[objectiveName] = requiredAmount;
            CurrentProgress[objectiveName] = 0;
        }
        public void AddItemReward(Item item)
        {
            ItemRewards.Add(item);
        }
        public void Start()
        {
            if (Status == QuestStatus.NotStarted)
            {
                Status = QuestStatus.Active;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n[!] Начат новый квест: {Name}");
                Console.ResetColor();
            }
        }
        public void UpdateProgress(string objectiveName, int amount)
        {
            if (Status != QuestStatus.Active || !Objectives.ContainsKey(objectiveName)) { return; }

            CurrentProgress[objectiveName] += amount;

            if (CurrentProgress[objectiveName] > Objectives[objectiveName])
            {
                CurrentProgress[objectiveName] = Objectives[objectiveName];
            }

            bool allCompleted = true;
            foreach (var objective in Objectives)
            {
                if (CurrentProgress[objective.Key] < objective.Value)
                {
                    allCompleted = false;
                    break;
                }
            }
            if (allCompleted)
            {
                Status = QuestStatus.Completed;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n[!] Квест: \"{Name}\" выполнен! Вернитесь к {QuestGiver.Name} за наградой.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n[!] Обновление квеста: {objectiveName} - {CurrentProgress[objectiveName]}/{Objectives[objectiveName]}");
                Console.ResetColor();
            }
        }
        public void Complete(Player player)
        {
            if (Status != QuestStatus.Completed) { return; }

            player.Gold += GoldReward;
            player.AddExperience(ExpReward);
            foreach (Item item in ItemRewards)
            {
                player.AddItem(item);
            }

            Status = QuestStatus.Finished;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n=== Квест завершен: {Name} ===");
            Console.WriteLine($"Получено золота: {GoldReward}");
            Console.WriteLine($"Получено опыта: {ExpReward}");

            if (ItemRewards.Count > 0)
            {
                Console.WriteLine("Полученные предметы: ");
                foreach (Item item in ItemRewards)
                {
                    Console.WriteLine($"- {item.Name}");
                }
            }
            Console.ResetColor();
        }
        public bool IsReadyToComplete()
        {
            return Status == QuestStatus.Completed;
        }
        public string GetProgressString()
        {
            string progress = "";
            foreach (var objective in Objectives)
            {
                progress += $"\n- {objective.Key}: {CurrentProgress[objective.Key]}/{objective.Value}";
            }
            return progress;
        }
    }
}
