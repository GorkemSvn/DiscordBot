using System;
using System.Collections.Generic;
using System.Text;
using Rpg;

namespace DiscordBot
{
    class Spawner
    {

        public static Mob Wolf(Village village)
        {
            var wolf = new Mob(5f, 10f, 0f, 2, 1, 0);
            wolf.name = "Wolf";
            wolf.SetVillage(village);
            return wolf;
        }
        public static Mob Bandit(Village village)
        {
            var bandit = new Mob(11f, 10f, 0f, 3, 1, 0);
            bandit.name = "Bandit";
            bandit.SetVillage(village);
            return bandit;
        }
    }
}
