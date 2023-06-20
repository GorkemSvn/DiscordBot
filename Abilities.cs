using System;
using System.Collections.Generic;
using System.Text;

namespace Rpg
{

    [Serializable]
    public abstract class Ability
    {
        public string name;
        protected object source;

        public Atribute power { get; protected set; }

        public Ability(object user)
        {
            this.source = user;
        }

        public void Perform(object targets)
        {
            if (CheckRequirements())
            {
                UseResources();
                Effect(targets);
            }
        }

        protected abstract bool CheckRequirements();
        protected abstract void UseResources();
        protected abstract void Effect(object target);
    }

    public class Skill:Ability
    {
        public float staminaCost;
        protected Character character;

        public Skill(Character character):base(character)
        {
            this.character = character;
        }
        protected override bool CheckRequirements()
        {
            if(character!=null)
            {
                if (character.stats.stamina.energy > staminaCost)
                    return true;
            }
            return false;
        }
        protected override void UseResources()
        {
            character.stats.stamina.Alter(-staminaCost);
        }

        protected override void Effect(object target)
        {
            throw new NotImplementedException();
        }
    }

    public class Spell : Ability
    {
        public float manaCost;

        protected Character character;

        public Spell(Character character) : base(character)
        {
            this.character = character;
        }

        protected override bool CheckRequirements()
        {
            if (character != null)
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
            character.stats.mana.Alter(-manaCost);
        }
    }
    public class DamageSpell : Spell
    {
        public float damage;
        public DamageSpell(Character character,float damage,float manaCost) : base(character)
        {
            this.damage = damage;
            this.manaCost = manaCost;
        }

        protected override void Effect(object target)
        {
            if(target is Character trgtChr)
            {
                var magicDamage = new Damage(damage, Damage.Type.magical, character);
                trgtChr.ReceiveDamage(magicDamage);
            }
        }
    }

    public static class AbilityGenerator
    {
        public static bool TeachFireBall(Character learner)
        {
            var fb = new DamageSpell(learner,10f,10f);
            return learner.skills.TryToLearn(fb);
        }
    }


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

    }
}
