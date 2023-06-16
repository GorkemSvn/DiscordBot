using System;
using System.Collections.Generic;
using System.Text;
using Rpg;

namespace DiscordBot
{
    public static class Crafting
    {
        public static Item wood,stone;
        static List<Recipe> recipes;

        public static List<Recipe> CheckCraftables(Inventory inventory)
        {
            if (recipes == null)
                BuildRecipes();

            var items = inventory.GetVirtualList();
            var recips = new List<Recipe>();
            foreach (var recipe in recipes)
            {
                if (recipe.Craftability(inventory))
                    recips.Add(recipe);
            }
            return recips;
        }

        public static void BuildRecipes()
        {
            recipes = new List<Recipe>();
            if (wood == null)
                BuildItems();

            var stoneClub = new Weapon(0.1f);
            stoneClub.SetStats(1.5f, 1f, 1f, 0f, 1f, 0f);
            stoneClub.name = "Stone Club";

            var stoneClubRecipe = new Recipe();
            stoneClubRecipe.craft = stoneClub;
            stoneClubRecipe.requirements = new List<Item>() { wood, stone };

            recipes.Add(stoneClubRecipe);

            var bow = new Weapon(0.75f);
            bow.agiFactor = 2f;
            bow.agiBonus = 1f;
            bow.strBonus = 1f;
            bow.name = "Wood Bow";
            var bowRecipe = new Recipe();
            bowRecipe.craft = bow;
            var woodDup = wood.Duplicate();
            woodDup.quantity = 4;
            bowRecipe.requirements = new List<Item>() { woodDup };

            recipes.Add(bowRecipe);
        }

        public static void BuildItems()
        {
            wood = new Item();
            wood.name = "Wood";
            wood.maxQuantity = 50;

            stone = new Item();
            stone.name = "Stone";
            stone.maxQuantity = 50;
        }

        public static Recipe GetRecipe(string craftName)
        {
            foreach (var item in recipes)
            {
                if (item.craft.name == craftName)
                    return item;
            }
            return null;
        }
    }
}
