using System;
using System.Collections.Generic;
using System.Text;

namespace Rpg
{
    //Maximum information exposing. A principle favors exposing internal parameters as much as it's not causing disfunctionality
    [Serializable]
    public class EnergyPool
    {
        public float energy { get; private set; }
        public float capasity { get; private set; }

        public float growRatio = 0.01f;

        private float coreCapasity;

        Dictionary<string, float> modifiers = new Dictionary<string, float>();
        Dictionary<string, float> multipliers = new Dictionary<string, float>();

        public EnergyPool(float cap)
        {
            coreCapasity = cap;
            CalculateCapasity();
            energy = capasity;
        }

        public void Alter(float change)
        {
            change = Math.Clamp(energy + change, 0, capasity) - energy;
            energy += change;
            coreCapasity += Math.Max(0f, -change) * growRatio;
            CalculateCapasity();
        }

        public void SetFactors(string key, float modifier, float multiplier)
        {
            if (!modifiers.ContainsKey(key))
                modifiers.Add(key, 0f);
            if (!multipliers.ContainsKey(key))
                multipliers.Add(key, 1f);

            modifiers[key] = modifier;
            multipliers[key] = multiplier;

            CalculateCapasity();
        }
        private void CalculateCapasity()
        {
            float sum = coreCapasity;

            foreach (var x in modifiers)
                sum += x.Value;

            foreach (var x in multipliers)
                sum *= x.Value;

            capasity= (float)Math.Round(sum, 2);
            energy= (float)Math.Round(energy, 2);
        }
        public delegate void EnergyPoolAction(float x);
    }

    [Serializable]
    public class Atribute
    {
        public float level { get; private set; }
        public float exp { get; private set; }
        public float expCap { get; private set; }

        public float growRatio = 0.0f;

        private int baseLevel;

        Dictionary<string, float> modifiers = new Dictionary<string, float>();
        Dictionary<string, float> multipliers = new Dictionary<string, float>();


        public Atribute(int startLevel, float expCapofLvl1 = 100f)
        {
            baseLevel = startLevel;
            expCap = expCapofLvl1 * (float)Math.Pow(1f + growRatio, startLevel - 1);

            exp = 0;
            CalculateCapasity();
        }

        public void SetFactors(string key, float modifier, float multiplier)
        {
            if (!modifiers.ContainsKey(key))
                modifiers.Add(key, 0f);
            if (!multipliers.ContainsKey(key))
                multipliers.Add(key, 1f);

            modifiers[key] = modifier;
            multipliers[key] = multiplier;
            CalculateCapasity();
        }

        public void ChangeExp(float change)
        {
            exp += change;

            while (exp > expCap)
            {
                exp -= expCap;
                expCap += expCap * growRatio;
                baseLevel++;
            }

            while (exp < 0)
            {
                exp += expCap;
                expCap -= expCap * growRatio;
                expCap = Math.Max(expCap, 1f);
                baseLevel--;
            }
            CalculateCapasity();
        }

        private void CalculateCapasity()
        {
            float sum = baseLevel;

            foreach (var x in modifiers)
                sum += x.Value;

            foreach (var x in multipliers)
                sum *= x.Value;

            level = (float)Math.Round(sum, 2);
        }
    }
    [Serializable]
    public class Item
    {
        public string name;
        public string info;
        public int quantity;
        public int value;
    }

    public struct Damage
    {
        public float magnitude;
        public Type type;
        public object source;

        public Damage(float magnitude, Type type, object source = null)
        {
            this.magnitude =(float)Math.Round( magnitude,2);
            this.type = type;
            this.source = source;
        }
        public enum Type { physical, magical }
    }
    [Serializable]
    public abstract class Ability
    {
        public string name;
        object source;
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

    [Serializable]
    public class Inventory
    {
        public int size = 20;
        HashSet<Item> items = new HashSet<Item>();

        public bool Add(Item item)
        {
            if (item == null)
                return false;

            if (items.Count > size)
                return false;

            if (items.Contains(item))
                return false;

            items.Add(item);
            return true;
        }

        public void Remove(Item item)
        {
            if (items.Contains(item))
                items.Remove(item);
        }
        public List<Item> GetVirtualList()
        {
            List<Item> virtualItems = new List<Item>();

            virtualItems.AddRange(items);

            return virtualItems;
        }
    }

    [Serializable]
    public class Recipe
    {
        public List<Item> requirements;
        public Item craft;

        public bool Craft(Inventory inventory)
        {
            if (requirements == null || craft == null)
                return false;

            if (Craftability(inventory))
                RemoveRequirements(inventory);

            inventory.Add(craft);
            return false;
        }

        public bool Craftability(Inventory inventory)
        {
            List<Item> items = inventory.GetVirtualList();

            foreach (Item requirement in requirements)
            {
                bool found = false;

                //search if you can find it;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].name == requirement.name)
                    {
                        found = true;
                        items.RemoveAt(i);
                        break;
                    }
                }

                if (!found)
                    return false;
            }

            return true;
        }
        void RemoveRequirements(Inventory inventory)
        {
            List<Item> items = inventory.GetVirtualList();

            foreach (Item requirement in requirements)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].name == requirement.name)
                    {
                        inventory.Remove(items[i]);
                        items.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }

    [Serializable]
    public class Stats
    {
        public EnergyPool health { get; private set; }
        public EnergyPool stamina { get; private set; }
        public EnergyPool mana { get; private set; }

        public Atribute strenght { get; private set; }
        public Atribute agility { get; private set; }
        public Atribute wisdom { get; private set; }

        public Stats(float hp, float sta, float mna, int str, int agi, int wis)
        {
            health = new EnergyPool(hp);
            stamina = new EnergyPool(sta);
            mana = new EnergyPool(mna);

            strenght = new Atribute(str);
            agility = new Atribute(agi);
            wisdom = new Atribute(wis);
        }
    }

    [Serializable]
    public class SkillBook
    {
        List<Ability> skills;

        public SkillBook()
        {
            skills = new List<Ability>();
        }

        public void Add(Ability skill)
        {
            if (!skills.Contains(skill) && skill != null)
                skills.Add(skill);
        }

        public List<Ability> GiveVirtualList()
        {
            List<Ability> vl = new List<Ability>();
            (vl).AddRange(skills);
            return vl;
        }

    }

    [Serializable]
    public class Equipments
    {
        public enum Placement { head, torso, leg, hands, weapon, shield, neck, finger };
        public Dictionary<Placement, Equipment> pieces;

        public float physicalDefence { get; private set; }
        public float magicalDefence { get; private set; }
        Character character;

        public Equipments(Character character)
        {
            this.character = character;
            pieces = new Dictionary<Placement, Equipment>();
            CalculateDefences();
        }

        public Item EquipAndGetDiscarded(Equipment piece)
        {
            Equipment itemTobeReturned = null;
            if (pieces.ContainsKey(piece.placement))
            {
                itemTobeReturned = pieces[piece.placement];
                itemTobeReturned?.OnDiscard(character);
            }

            pieces[piece.placement] = piece;
            piece.OnEquip(character);
            CalculateDefences();
            return itemTobeReturned;
        }
        public float FilterDamage(Damage damage)
        {
            float defence = 0;
            var eqps = pieces.Values;
            foreach (Equipment equipment in eqps)
            {
                defence += equipment.DefencePower(damage);
            }
            defence += character.stats.agility.level;

            float reducedDamage = RelativeDamageReduction(damage.magnitude, defence);
            return reducedDamage;
        }

        static float RelativeDamageReduction(float damage, float defence)
        {
            //damage is quarter if damage is equal to defence

            if (damage > 0f)
            {
                float DefenceFactor = 1f + Math.Max(0, defence) / damage;
                damage /= (float)Math.Pow(DefenceFactor, 2);

                return Math.Max(0f, damage);
            }
            return 0;
        }

        void CalculateDefences()
        {
            var eqps = pieces.Values;
            physicalDefence = 0f;
            magicalDefence = 0f;
            foreach (Equipment equipment in eqps)
            {
                physicalDefence += equipment.DefencePower(new Damage(1f, Damage.Type.physical));
                magicalDefence += equipment.DefencePower(new Damage(1f, Damage.Type.magical));
            }
            physicalDefence += character.stats.agility.level;
            magicalDefence += character.stats.agility.level;
        }

        public class Equipment : Item
        {
            public List<Defence> defences { get; private set; }
            public Placement placement { get; private set; }

            public Equipment(List<Defence> defs, Placement placement)
            {
                defences = defs;
                this.placement = placement;
            }

            public virtual void OnEquip(Character character)
            {
                //attribute bonuses here
                //check compatibility
            }
            public virtual void OnDiscard(Character character)
            {
                //attribute bonuses here
                //check compatibility
            }

            public float DefencePower(Damage damage)
            {
                foreach (Defence d in defences)
                {
                    if (d.type == damage.type)
                        return d.power;
                }
                return 0f;
            }

            public struct Defence
            {
                public Damage.Type type;
                public float power;
            }
        }
    }

    [Serializable]
    public class Weapon : Equipments.Equipment
    {
        public float strFactor=1f, agiFactor=1f,wisFactor=1f;

        public float strBonus, agiBonus,wisBonus;

        public float balance { get; private set; }//well balanced weapons give agility exp


        public Weapon(float balance):base(new List<Defence>() { },Equipments.Placement.weapon)
        {
            this.balance = Math.Clamp(balance, 0f, 1f);
        }
        public override void OnEquip(Character character)
        {
            character.stats.strenght.SetFactors("weapon", strBonus, strFactor);
            character.stats.agility.SetFactors("weapon", agiBonus, agiFactor);
            character.stats.wisdom.SetFactors("weapon", wisBonus, wisFactor);
            base.OnEquip(character);
        }
        public override void OnDiscard(Character character)
        {
            character.stats.strenght.SetFactors("weapon", 0, 1f);
            character.stats.agility.SetFactors("weapon", 0, 1f);
            character.stats.wisdom.SetFactors("weapon", 0, 1f);
            base.OnEquip(character);
        }


    }
}