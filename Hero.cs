using System;
using System.Collections.Generic;
using System.Text;
using DiscordBot;

namespace Rpg
{
    [System.Serializable]
    public class Hero:Character
    {
        public ulong id { get; private set; }

        public Hero(ulong Id,string name)
        {
            id = Id;
            this.name = name;
        }


    }



    [System.Serializable]
    public class Mob : Character
    {
        public float dropChance = 0.99f;
        public List<Item> drops = new List<Item>();
        int lifeTime = 0;
        public Mob(float hp, float sta, float mna, int str, int agi, int wis)
        {
            stats = new Stats(hp, sta, mna, str, agi, wis);
        }

        protected override void ExperienceSecond()
        {
            base.ExperienceSecond();
            if (++lifeTime > 3600)
            {
                village.SendMessage(name + " left");
                Destroy();
            }
        }

        protected override void OnDeath()
        {
            if (village == null)
                return;

            base.OnDeath();
            if (Program.random.NextDouble()<dropChance&& drops.Count > 0)
            {
                var drop = drops[Program.random.Next(0, drops.Count)];
                var dropCapsule = new ItemCapsule(drop);
                dropCapsule.SetVillage(village);
                village.SendMessage(drop.name+" is dropped");
            }

            Destroy();
        }
    }
}
