using System;
using System.Collections.Generic;
using System.Text;

namespace Rpg
{
    [Serializable]
    public class AbilityCollection
    {
        HashSet<Ability> skills;
        Character user;
        public AbilityCollection(Character character)
        {
            skills = new HashSet<Ability>();
            user = character;
        }

        public bool TryToLearn(Ability skill)
        {
            foreach (var item in skills)
            {
                if (skill.name == item.name)
                    return false;
            }
            if (!skills.Contains(skill) && skill != null)
            {
                skill.source = user;
                skills.Add(skill);
                return true;
            }

            return false;
        }

        public List<Ability> GiveVirtualList()
        {
            List<Ability> vl = new List<Ability>();
            (vl).AddRange(skills);
            return vl;
        }
        public Ability Find(string name)
        {
            foreach (var item in skills)
            {
                if (item.name == name)
                    return item;
            }
            return null;
        }

        public void Discard(Ability skill)
        {
            if (skills.Contains(skill))
                skills.Remove(skill);
        }
        public void Discard(string skillName)
        {
            var skill = Find(skillName);
            if (skill != null)
                Discard(skill);
        }
    }

    [Serializable]
    public abstract class Ability
    {
        public string name;
        public object source;

        public Atribute power { get; protected set; }

        public Ability()
        {
            power = new Atribute(1);
        }

        public bool Perform(object targets)
        {
            if (CheckRequirements())
            {
                UseResources();
                Effect(targets);
                power.ChangeExp(1f);
                return true;
            }
            return false;
        }

        public abstract bool CheckRequirements();
        protected abstract void UseResources();
        protected abstract void Effect(object target);
    }

    public static class AbilityGenerator
    {
        public static Ability FireBall { get
            {
                var fb = new DamageSpell( 10f, 10f);
                fb.name = "FireBall";
                return fb;
            } 
        }
        public static Ability Shock { get
            {
                var fb = new DamageSpell( 4f, 2f);
                fb.name = "Shock";
                return fb;
            } 
        }
        public static Ability HardHit { get
            {
                var fb = new Skill( 10f, 1f,1.25f);
                fb.name = "Hard Hit";
                return fb;
            } 
        }
        /*
        spells
        damaging spells, necromancy, heal, buff, curse,
        item conversion & produce, botanic, weather, summoning
        */
    }

    #region Skills
    [Serializable]
    public class Skill:Ability
    {
        public float staminaCost;
        float bonus, factor;
        public Skill(float cost, float damageBonus, float damageFactor)
        {
            bonus = damageBonus;
            factor = damageFactor;
            staminaCost = cost;
        }
        public override bool CheckRequirements()
        {
            if(source is Character character)
            {
                if (character.stats.stamina.energy >= staminaCost)
                    return true;
            }
            return false;
        }
        protected override void UseResources()
        {
            (source as Character).stats.stamina.Alter(-staminaCost);
        }

        protected override void Effect(object target)
        {
            if(target is Character trg)
            {
                var str = (source as Character).stats.strenght.level + bonus + power.level;
                trg.ReceiveDamage(new Damage(str * factor, Damage.Type.physical, source));
            }
        }
    }
    #endregion
    //**********************************
    #region Spells
    [Serializable]
    public class Spell : Ability
    {
        public float manaCost;


        public override bool CheckRequirements()
        {
            if (source is Character character)
            {
                if (character.stats.stamina.energy > manaCost)
                    return true;
            }
            return false;
        }

        protected override void Effect(object target)
        {
            throw new NotImplementedException();
        }

        protected override void UseResources()
        {
            (source as Character).stats.mana.Alter(-manaCost);
        }
    }

    [Serializable]
    public class DamageSpell : Spell
    {
        public float damage;
        public DamageSpell(float damage,float manaCost)
        {
            this.damage = damage;
            this.manaCost = manaCost;
        }

        protected override void Effect(object target)
        {
            if(target is Character trgtChr)
            {
                var magicDamage = new Damage(damage, Damage.Type.magical, source);
                trgtChr.ReceiveDamage(magicDamage);
            }
        }
    }
    #endregion

}
