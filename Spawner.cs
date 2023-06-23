using System;
using System.Collections.Generic;
using System.Text;
using Rpg;

namespace DiscordBot
{
    class Spawner
    {

        public static Mob Boar(Village village)
        {
            var boar = new Mob(6f, 2f, 0f, 2, 0, 0);
            boar.name = "Boar";
            boar.drops.Add(ItemGenerator.meat);
            boar.SetVillage(village);
            return boar;
        }
        public static Mob Brawler(Village village)
        {
            var brawler = new Mob(8f, 10f, 0f, 2, 0, 0);
            brawler.name = "Brawler";
            brawler.drops.Add(ItemGenerator.HardHitBook);
            brawler.skills.TryToLearn(AbilityGenerator.HardHit);
            brawler.SetVillage(village);
            return brawler;
        }
        public static Mob Bandit(Village village)
        {
            var bandit = new Mob(12f, 10f, 0f, 6, 2, 0);
            bandit.name = "Bandit";
            bandit.drops.Add(ItemGenerator.BluntSword);
            bandit.drops.Add(ItemGenerator.Bow);
            bandit.drops.Add(ItemGenerator.Pendant);
            bandit.SetVillage(village);
            return bandit;
        }

        public static Mob NoviceWitch(Village village)
        {
            var witch = new Mob(14f, 10f, 10f, 1, 2, 25);
            witch.name = "Novice Witch";
            witch.skills.TryToLearn(AbilityGenerator.FireBall);
            witch.skills.TryToLearn(AbilityGenerator.Shock);
            witch.drops.Add(ItemGenerator.FireBallBook);
            witch.drops.Add(ItemGenerator.ShockBook);
            witch.SetVillage(village);
            return witch;
        }
    }
}
