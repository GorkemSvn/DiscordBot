using System;
using System.Collections.Generic;
using System.Text;
using DiscordBot;

namespace Rpg
{
    public class Hero:Character
    {
        public ulong id { get; private set; }
        public string name { get; private set; }
        public Hero(ulong Id,string name)
        {
            id = Id;
            this.name = name;
        }


    }



    public class Mob : Character
    {
        public Mob(float hp, float sta, float mna, int str, int agi, int wis)
        {
            stats = new Stats(hp, sta, mna, str, agi, wis);
        }

        protected override void ExperienceSecond()
        {
            
        }
    }
}
