using System;
using System.Collections.Generic;
using System.Text;
using DiscordBot;

namespace Rpg
{
    [System.Serializable]
    public class Character : Village.Object
    {
        public Stats stats { get; protected set; }
        public Inventory inventory { get; protected set; }
        public Equipments equipments { get; protected set; }
        public AbilityCollection skills { get; protected set; }

        protected Character target;

        public Character()
        {
            stats = new Stats(8f, 10f, 0f, 2, 0, 0);
            inventory = new Inventory();
            equipments = new Equipments(this);
            skills = new AbilityCollection(this);
        }

        protected override void ExperienceSecond()
        {
            stats.health.Alter(0.1f);
            stats.stamina.Alter(0.1f);
            stats.mana.Alter(0.1f);

            stats.health.bonus.SetFactors("strenght", stats.strenght.level, 1f);
            stats.stamina.bonus.SetFactors("agility", stats.agility.level, 1f);
            stats.mana.bonus.SetFactors("wisdom", stats.wisdom.level, 1f);

            if (target != null)
            {
                if(!TryRandomSkill())
                    Hit();
            } 
        }

        public void Attack(Character target)
        {
            this.target = target;
        }

        void Hit()
        {
            float baseD = stats.strenght.level;

            float balance = 0.5f;
            if (equipments.pieces.ContainsKey(Equipments.Placement.weapon))
            {
                var weapon = equipments.pieces[Equipments.Placement.weapon] as Weapon;
                balance = weapon.balance;
            }

            stats.agility.ChangeExp(balance);
            stats.strenght.ChangeExp(1f - balance);


            target?.ReceiveDamage(new Damage(baseD, Damage.Type.physical, this));

            if (target!=null &&target.stats.health.energy <= 0f)
                target = null;
        }
        bool TryRandomSkill()
        {
            var skillsList = skills.GiveVirtualList();
            if (skillsList.Count > 0)
            {
                var selectedSkill = skillsList[Program.random.Next(0, skillsList.Count)];
                if(selectedSkill.CheckRequirements())
                {
                    village.AddToLog(name + " used " + selectedSkill.name);
                    selectedSkill.Perform(target);
                    if (selectedSkill is Skill)
                        stats.agility.ChangeExp(1f);
                    else if (selectedSkill is Spell)
                        stats.wisdom.ChangeExp(1f);

                    return true;
                }
            }
            return false;
        }

        public virtual void ReceiveDamage(Damage damage)
        {
            if (damage.source != null && damage.source is Character)//is able to fight back
                target = damage.source as Character;

            float d = equipments.FilterDamage(damage);
            stats.health.Alter(-d);
            village?.AddToLog(name + " received " +Math.Round( d,2) + " damage. (Health " + stats.health.energy + ")   ");
            
            if (stats.health.energy <= 0f)
            {
                OnDeath();
                target = null;
            }
        }

        protected virtual void OnDeath()
        {
            village?.AddToLog(name + " lost.");
            village?.SendLogs();
        }


        public delegate void CharacterAction(Character character);
    }
}
