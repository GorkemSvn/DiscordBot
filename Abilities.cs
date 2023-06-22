using System;
using System.Collections.Generic;
using System.Text;

namespace Rpg
{
    [Serializable]
    public class SkillBook
    {
        HashSet<Ability> skills;

        public SkillBook()
        {
            skills = new HashSet<Ability>();
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

        protected abstract bool CheckRequirements();
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
                var fb = new Skill( 10f, 2f,2f);
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
        protected override bool CheckRequirements()
        {
            if(source is Character character)
            {
                if (character.stats.stamina.energy > staminaCost)
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
                var str = (source as Character).stats.strenght.level;
                trg.ReceiveDamage(new Damage((str+power.level) * bonus * factor, Damage.Type.physical, source));
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


        protected override bool CheckRequirements()
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
