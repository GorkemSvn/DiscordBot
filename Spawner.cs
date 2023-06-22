using System;
using System.Collections.Generic;
using System.Text;
using Rpg;

namespace DiscordBot
{
    class Spawner
    {

        public static Mob Coyote(Village village)
        {
            var wolf = new Mob(5f, 10f, 0f, 2, 1, 0);
            wolf.name = "Coyote";
            wolf.SetVillage(village);
            return wolf;
        }
        public static Mob Brawler(Village village)
        {
            var bandit = new Mob(6f, 10f, 0f, 2, 1, 0);
            bandit.name = "Brawler";
            bandit.drops.Add(ItemGenerator.HardHitBook);
            bandit.SetVillage(village);
            return bandit;
        }
        public static Mob Bandit(Village village)
        {
            var bandit = new Mob(15f, 10f, 0f, 6, 2, 0);
            bandit.name = "Bandit";
            bandit.drops.Add(ItemGenerator.BluntSword);
            bandit.drops.Add(ItemGenerator.Bow);
            bandit.drops.Add(ItemGenerator.Pendant);
            bandit.SetVillage(village);
            return bandit;
        }

        public static Mob NoviceWitch(Village village)
        {
            var witch = new Mob(10f, 10f, 10f, 1, 2, 5);
            witch.name = "Novice Witch";
            witch.drops.Add(ItemGenerator.FireBallBook);
            witch.drops.Add(ItemGenerator.ShockBook);
            witch.SetVillage(village);
            return witch;
        }
    }
}
