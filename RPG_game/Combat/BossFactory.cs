using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_game
{
    public static class BossFactory
    {
        public static Boss CreateBoss(string bossName, int playerLevel)
        {
            switch (bossName.ToLower())
            {
                case "лесной гигант":
                    return CreateForestGiant(playerLevel);

                case "королева пауков":
                    return CreateSpiderQueen(playerLevel);

                case "древний страж":
                    return CreateAncientGuardin(playerLevel);

                default:
                    return CreateGenericBoss(bossName, playerLevel);
            }
        }

        public static Boss CreateForestGiant(int playerLevel)
        {
            int bossLevel = playerLevel + 2;
            Boss boss = new Boss("Древень", "Огромное древоподобное существо, чьи корни уходят глубоко в землю.", "Лесной Гигант", bossLevel);
            boss.PhaceCount = 2;
            boss.AddGuaranteedLoot(new Weapon("Посох Природы", "Волшебный посох, созданный из веток древнего дерева.", 10 + playerLevel, 100 + playerLevel * 10));
            boss.AddGuaranteedLoot(new Item("Сердцевина Древня", "Магическая сущность, пульсирующая энергией леса", 200));

            return boss;
        }

        public static Boss CreateSpiderQueen(int playerLevel)
        {
            int bossLevel = playerLevel + 3;
            Boss boss = new Boss("Арахния", "Огромная паучиха с восемью глазами, сверкающими в темноте пещеры.", "Королева Пауков", bossLevel);
            boss.PhaceCount = 2;
            boss.AddGuaranteedLoot(new Armor("Хитиновый доспех", "Доспех, созданный из хитина гигантского паука.", 7 + playerLevel/2, 120 + playerLevel * 10));
            boss.AddGuaranteedLoot(new Item("Ядовитая железа", "Железа, королевы пауков, содержит смертельный яд", 180));

            return boss;
        }

        public static Boss CreateAncientGuardin(int playerLevel)
        {
            int bossLevel = playerLevel + 5;
            Boss boss = new Boss("Голем", "Массивная каменная конструкция, созданная древней цивилизацией для охраны сокровищ.", "Древний Страж", bossLevel);
            boss.PhaceCount = 3;
            boss.AddGuaranteedLoot(new Weapon("Меч Титана", "Огромный меч, который под силу поднять лишь истинному герою", 15 + playerLevel, 300 + playerLevel * 15));
            boss.AddGuaranteedLoot(new Armor("Нагрудник Стража", "Нагрудник из неизвестного металла, практически неразрушимый.", 10 + playerLevel / 2, 250 + playerLevel * 15));
            boss.AddGuaranteedLoot(new Item("Древний кристалл", "Источник силы, питавший древнего стража.", 500));

            return boss;
        }

        private static Boss CreateGenericBoss(string bossName, int playerLevel)
        {
            throw new NotImplementedException();
        }
    }
}
