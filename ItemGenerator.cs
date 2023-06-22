using System;
using System.Collections.Generic;
using System.Text;

namespace Rpg
{
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
            if (newItem.quantity > 0)
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
    public class Item
    {
        public string name;
        public string info;
        public int quantity = 1;
        public int maxQuantity = 1;
        public int value;


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
        public virtual bool Use(object _on)
        {
            return false;
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
    static class ItemGenerator
    {
        public static Item wood
        {
            get
            {
                var w = new Item();
                w.name = "Wood";
                w.maxQuantity = 50;
                return w;
            }
        }
        public static Item stone
        {
            get
            {
                var w = new Item();
                w.name = "Stone";
                w.maxQuantity = 50;
                return w;
            }
        }
        public static Item Bow
        {
            get
            {
                var w = new Weapon(0.8f);
                w.name = "Wood Bow";
                w.maxQuantity = 1;
                w.agiBonus = 2;
                return w;
            }
        }
        public static Item BluntSword
        {
            get
            {
                var w = new Weapon(0.5f);
                w.name = "Blunt Sword";
                w.maxQuantity = 1;
                w.strBonus = 1;
                w.agiBonus = 1;
                return w;
            }
        }
        public static Item WoodClub
        {
            get
            {
                var w = new Weapon(0.25f);
                w.name = "Wood Club";
                w.maxQuantity = 1;
                w.strBonus = 2;
                w.agiBonus = 0;
                return w;
            }
        }
        public static Item Pendant
        {
            get
            {
                var w = new Equipments.Equipment(null,Equipments.Placement.neck);
                w.name = "Pendant";
                w.maxQuantity = 1;
                w.wisBonus = 3;

                return w;
            }
        }

        public static Item FireBallBook
        {
            get
            {
                var w = new Book(AbilityGenerator.FireBall);
                w.name = "MagicBook: FireBall";
                w.maxQuantity = 1;
                return w;
            }
        }
        public static Item ShockBook
        {
            get
            {
                var w = new Book(AbilityGenerator.Shock);
                w.name = "MagicBook: Shock";
                w.maxQuantity = 1;
                return w;
            }
        }
        public static Item HardHitBook
        {
            get
            {
                var w = new Book(AbilityGenerator.HardHit);
                w.name = "SkillBook: Hard Hit";
                w.maxQuantity = 1;
                return w;
            }
        }


        static Item BasicItem(string name,int quantity)
        {
            var w = new Item();
            w.name = name;
            w.maxQuantity = quantity;
            return w;
        }
    }

    [Serializable]
    public class Book:Item
    {
        Ability ability;

        public Book(Ability teach)
        {
            ability = teach;
        }
        public override bool Use(object _on)
        {
            if(_on is Character chrct)
            {
                chrct.inventory.Remove(this);
                ability.source = _on ;
                return chrct.skills.TryToLearn(ability);
            }
            return false;
        }
    }
}
