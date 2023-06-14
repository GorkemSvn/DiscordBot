﻿using System;
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
        public SkillBook skills { get; protected set; }

        protected Character target;

        public Character()
        {
            stats = new Stats(10f, 10f, 0f, 2, 1, 1);
            inventory = new Inventory();
            equipments = new Equipments(this);
            skills = new SkillBook();
        }

        protected override void ExperienceSecond()
        {
            stats.health.Alter(0.1f);
            stats.stamina.Alter(0.1f);
            stats.mana.Alter(0.1f);

            if (target != null) 
            {
                if (target.stats.health.energy > 0)
                    Hit();

                target = null;
            } 
        }

        public void Attack(Character target)
        {
            if (stats.health.energy == 0)
                return;

            this.target = target;
        }

        void Hit()
        {
            float baseD = stats.strenght.level;
            baseD *= 1f + stats.agility.level / 100f;

            float balance = 0.5f;
            if (equipments.pieces.ContainsKey(Equipments.Placement.weapon))
            {
                var weapon = equipments.pieces[Equipments.Placement.weapon] as Weapon;
                balance = weapon.balance;
            }

            stats.agility.ChangeExp(balance);
            stats.strenght.ChangeExp(1f - balance);

            stats.health.SetFactors("strenght", stats.strenght.level, 1f);

            target.ReceiveDamage(new Damage(baseD, Damage.Type.physical, this));
        }


        public virtual void ReceiveDamage(Damage damage)
        {

            float d = equipments.FilterDamage(damage);
            stats.health.Alter(-d);
            village?.SendMessage(name + " received " + d + " damage. (Health :"+stats.health.energy+")");
            if (stats.health.energy == 0)
                OnDeath();
        }

        protected virtual void OnDeath()
        {

        }


        public delegate void CharacterAction(Character character);
    }
}
