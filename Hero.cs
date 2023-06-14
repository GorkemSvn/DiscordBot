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

        public override void ReceiveDamage(Damage damage)
        {
            if (damage.source != null && damage.source is Character)
                target = damage.source as Character;
            base.ReceiveDamage(damage);
        }

        protected override void OnDeath()
        {
            village.SendMessage(name + " is dead");
            
            foreach (var item in drops)
            {
                var capsule=new ItemCapsule(item);
                capsule.SetVillage(village);
            }
            Destroy();
        }
    }
}
