using System;
using System.Collections.Generic;
using System.Text;
using DiscordBot;

namespace Rpg
{
    public class Character : Village.Object
    {
        public Stats stats { get; protected set; }
        public Inventory inventory { get; protected set; }
        public Equipments equipments { get; protected set; }
        public SkillBook skills { get; protected set; }

        public Character()
        {
            stats = new Stats(10f, 10f, 0f, 1, 1, 1);
            inventory = new Inventory();
            equipments = new Equipments(this);
            skills = new SkillBook();
        }

        protected override void ExperienceSecond()
        {
            stats.health.Alter(1f);
            stats.stamina.Alter(1f);
            stats.mana.Alter(1f);
        }

        public void Attack(Character target)
        {
            float baseD = stats.strenght.level;
            baseD *= 1f + stats.agility.level / 100f;

            if (equipments.pieces.ContainsKey(Equipments.Placement.weapon))
            {
                var weapon = equipments.pieces[Equipments.Placement.weapon] as Weapon;
                stats.agility.ChangeExp(weapon.balance);
                stats.strenght.ChangeExp(1f - weapon.balance);

                stats.health.SetFactors("strenght", stats.strenght.level, 1f);
            }

            target.ReceiveDamage(new Damage(baseD, Damage.Type.physical, this));
        }

        public void ReceiveDamage(Damage damage)
        {
            float d = equipments.FilterDamage(damage);
            stats.health.Alter(-d);
        }


        public delegate void CharacterAction(Character character);
    }
}
