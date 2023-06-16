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
        public List<Item> drops = new List<Item>();
        public Mob(float hp, float sta, float mna, int str, int agi, int wis)
        {
            stats = new Stats(hp, sta, mna, str, agi, wis);
        }


        protected override void OnDeath()
        {
            if (village == null)
                return;

            base.OnDeath();
            foreach (var item in drops)
            {
                target.inventory.Add(item);
                village.SendMessage(target.name+" took " + item.name);
            }
            Destroy();
        }
    }
}
