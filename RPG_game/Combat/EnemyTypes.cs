using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public class EnemyTypes
    {
    }

    public static class EnemyFactory
    {
        private static Random Random = new Random();

        public static Enemy CreateRandomEnemy(string location, int playerLevel)
        {
            int minLevel = Math.Max(1, playerLevel - 2);
            int maxLevel = playerLevel + 2;
            int enemyLevel = Random.Next(minLevel, maxLevel + 1);

            switch (location.ToLower())
            {
                case "лес":
                    return CreateForestEnemy(enemyLevel);
                case "пещера":
                    return CreateCaveEnemy(enemyLevel);
                case "подземелье":
                    return CreateDungeonEnemy(enemyLevel);
                default:
                    return CreateCommonEnemy(enemyLevel);
                        
            }
        }

        private static Enemy CreateForestEnemy(int level)
        {
            string[] enemies =
            {
                "Волк|Дикий волк с острыми клыками.|1.2|0.8",
                "Медведь|Большуй бурый мишка.|1.5|1.2",
                "Разбойник|Бандит, скрывающийся в лесу.|1.0|1.0",
                "Гоблин?|Маленькое зеленое существо с кинжалом.|0.8|0.7",
                "Огромный паук...|Паук с размером с три медведя...|0.9|0.6"
            };

            return CreateEnemyFromTemplate(enemies, level);
        }

        private static Enemy CreateCaveEnemy(int level)
        {
            string[] enemies =
            {
                "Летучая мышь|Быстрая и назойливая.|0.7|0.5",
                "Троглодит?|Пещерное существо с... дубиной...???|1.1|0.9",
                "Слизь|Желеобразное существо, которое медленно двигается.|0.8|1.2",
                "Скелет(Костян)|Оживший скелет с мечом.|1.0|0.8",
                "Медведь-пещерник|Огромный мишка, обитающий в пещере.|1.6|1.3"
            };

            return CreateEnemyFromTemplate(enemies, level);
        }

        private static Enemy CreateDungeonEnemy(int level)
        {
            string[] enemies =
            {
                "Скелет-воин|Скелет в древних доспехах.|1.0|1.2",
                "Зомби|Медленный, но сильный неживой противник.|1.2|0.7",
                "Призрак|Полупрозрачная сущность, которую трудно поразить.|0.8|1.4",
                "Гоблин-шаман|Гоблин, владеющий темной магией.|1.1|0.9",
                "Минотавр...|Огромное существо с головой быка. (мне пи***)|1.8|1.5"
            };

            return CreateEnemyFromTemplate(enemies, level);
        }

        private static Enemy CreateCommonEnemy(int level)
        {
            string[] enemies =
            {
                "Бродяга|Грязный бродяга с ножом.|0.9|0.8",
                "Дикая собака|Одичавшая голодная собака.|1.0|0.7",
                "Крыса-мутант|Огромная крыса размером с кошку.|0.7|0.6",
                "Грабитель|Человек в маске с кинжалом.|1.1|0.9",
                "Ядовитая змея|Змея с опасными зубками.|0.8|0.7",
            };

            return CreateEnemyFromTemplate(enemies, level);
        }

        private static Enemy CreateEnemyFromTemplate(string[] templates, int level)
        {
            string template = templates[Random.Next(templates.Length)];
            string[] parts = template.Split('|');

            string name = parts[0];
            string description = parts[1];
            double attackMod = double.Parse(parts[2], CultureInfo.InvariantCulture);
            double defenseMod = double.Parse(parts[3], CultureInfo.InvariantCulture);

            Enemy enemy = new Enemy(name, description, level);

            enemy.AttackPower = (int)(enemy.AttackPower * attackMod);
            enemy.Defense = (int)(enemy.Defense * defenseMod);

            return enemy;
        }

        public static Enemy CreateBoss(string name, int playerLevel)
        {
            int bossLevel = playerLevel + 3;

            Enemy boss = new Enemy(
                name, $"БОСС: {name} - грозный противник, намного сильнее обычных врагов.",
                bossLevel
                );

            boss.MaxHealth *= 2;
            boss.Health = boss.MaxHealth;
            boss.AttackPower = (int)(boss.AttackPower * 1.5);
            boss.Defense = (int)(boss.Defense * 1.3);
            boss.ExpReward *= 3;
            boss.GoldReward *= 5;

            return boss;

        }
    }

}
