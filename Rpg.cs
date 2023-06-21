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
        public float capasity { get { return bonus.Calculate(coreCapasity); } }

        public Bonus bonus { get; private set; }

        public float growRatio = 0.01f;

        private float coreCapasity;


        public EnergyPool(float cap)
        {
            coreCapasity = cap;
            bonus = new Bonus();
            energy = capasity;
        }

        public void Alter(float change)
        {
            change = Math.Clamp(energy + change, 0, capasity) - energy;
            energy += change;
            coreCapasity += Math.Max(0f, -change) * growRatio;
            energy =(float) Math.Round(energy, 2);
        }

        public delegate void EnergyPoolAction(float x);
    }
    [Serializable]
    public class Bonus
    {
        public float modifier { get; private set; } = 0f;
        public float multiplier { get; private set; } = 1f;

        Dictionary<string, float> modifiers = new Dictionary<string, float>();
        Dictionary<string, float> multipliers = new Dictionary<string, float>();

        public void SetFactors(string key, float _modifier=1f, float _multiplier=1f)
        {
            if (!modifiers.ContainsKey(key))
                modifiers.Add(key, 0f);
            if (!multipliers.ContainsKey(key))
                multipliers.Add(key, 1f);

            modifiers[key] = _modifier;
            multipliers[key] = _multiplier;


            modifier = 0f;
            foreach (var x in modifiers)
                modifier += x.Value;

            multiplier = 1f;
            foreach (var x in multipliers)
                multiplier *= x.Value;
        }

        public float Calculate(float baseValue)
        {
            return (float)Math.Round( baseValue * multiplier + modifier,2);
        }
    }

    [Serializable]
    public class Atribute
    {
        public AtributeAction OnLevelUp;
        public float level { get { return bonus.Calculate(baseLevel); } }
        public float exp { get; private set; }
        public float expCap { get; private set; }
        public Bonus bonus { get; private set; }

        public float growRatio = 0.0f;

        int baseLevel;



        public Atribute(int startLevel, float expCapofLvl1 = 100f)
        {
            baseLevel = startLevel;
            expCap = expCapofLvl1 * (float)Math.Pow(1f + growRatio, startLevel - 1);
            bonus = new Bonus();
            exp = 0;
        }


        public void ChangeExp(float change)
        {
            exp += change;

            while (exp > expCap)
            {
                exp -= expCap;
                expCap += expCap * growRatio;
                baseLevel++;
                OnLevelUp?.Invoke();
            }

            while (exp < 0)
            {
                exp += expCap;
                expCap -= expCap * growRatio;
                expCap = Math.Max(expCap, 1f);
                baseLevel--;
            }
            exp =(float) Math.Round(exp, 2);
        }

        public delegate void AtributeAction();
    }
    [Serializable]
    public class Item
    {
        public string name;
        public string info;
        public int quantity=1;
        public int maxQuantity=1;
        public int value;

        public List<Item> requirements;

        public Item Duplicate()
        {
            var dup = new Item();
            dup.name = name;
            dup.info = info;
            dup.quantity = quantity;
            dup.value = value;
            dup.maxQuantity = maxQuantity;
            return dup;
        }

        public void Add(Item item)
        {
            if (item.name == name)
            {
                int emptySpace = maxQuantity - item.quantity;
                int addition = Math.Min(emptySpace, item.quantity);
                quantity += addition;
                item.quantity -= addition;
            }
        }
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
    public class Inventory
    {
        public int size = 20;
        HashSet<Item> items = new HashSet<Item>();

        public bool Add(Item newItem)
        {
            if (newItem == null)
                return false;

            if (items.Count > size)
                return false;

            if (items.Contains(newItem))
                return false;

            foreach (var item in items)
            {
                if (item.name == newItem.name)
                {
                    item.Add(newItem);
                    if (newItem.quantity < 1)
                        return true;
                }
            }
            if(newItem.quantity>0)
                items.Add(newItem);

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
            {
                RemoveRequirements(inventory);
                inventory.Add(craft);
                return true;
            }
            return false;
        }

        //checks names
        public bool Craftability(Inventory inventory)
        {
            List<Item> items = inventory.GetVirtualList();
            var ingredients = IngredientsDuplicate();

            for (int i = 0; i < ingredients.Count; i++)
            {
                var ingredient = ingredients[i];
                foreach (var item in items)//try to diminish ingredient in inventory
                {
                    if (item.name == ingredient.name)
                    {
                        var quantity = Math.Min(ingredient.quantity, item.quantity);
                        ingredient.quantity -= quantity;
                        if (ingredient.quantity <= 0)
                            ingredients.RemoveAt(i--);

                    }
                }
            }

            return ingredients.Count < 1;
        }
        void RemoveRequirements(Inventory inventory)
        {
            List<Item> items = inventory.GetVirtualList();
            var ingredients = IngredientsDuplicate();

            for (int i = 0; i < ingredients.Count; i++)
            {
                var ingredient = requirements[i];
                for (int j = 0; j < items.Count; j++)
                {
                    var item = items[j];
                    if (item.name == ingredient.name)
                    {
                        var quantity = Math.Min(ingredient.quantity, item.quantity);

                        item.quantity -= quantity;
                        ingredient.quantity -= quantity;
                        if (item.quantity == 0)
                            inventory.Remove(item);
                        if (ingredient.quantity == 0)
                            break;
                    }
                }
            }
        }
        List<Item> IngredientsDuplicate()
        {
            var ingredients = new List<Item>();

            foreach (var item in requirements)
            {
                ingredients.Add(item.Duplicate());
            }

            return ingredients;
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

        [Serializable]
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

            [Serializable]
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
            character.stats.strenght.bonus.SetFactors("weapon", strBonus, strFactor);
            character.stats.agility.bonus.SetFactors("weapon", agiBonus, agiFactor);
            character.stats.wisdom.bonus.SetFactors("weapon", wisBonus, wisFactor);
            base.OnEquip(character);
        }
        public override void OnDiscard(Character character)
        {
            character.stats.strenght.bonus.SetFactors("weapon", 0, 1f);
            character.stats.agility.bonus.SetFactors("weapon", 0, 1f);
            character.stats.wisdom.bonus.SetFactors("weapon", 0, 1f);
            base.OnEquip(character);
        }

        public void SetStats(float strF,float strB,float agiF,float agiB,float wisF,float wisB)
        {
            strFactor = strF;
            strBonus = strB;

            agiFactor = agiF;
            agiBonus = agiB;

            wisFactor = wisF;
            wisBonus = wisB;
        }

    }
}