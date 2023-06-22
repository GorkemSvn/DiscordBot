using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot
{
    
    class Program
    {
        public static Random random = new Random(DateTime.Now.Millisecond);
        static void Main(string[] args)
        {
            Crafting.BuildRecipes();
            var bot = new Bot();
            Server.Load();
            Server.SetActive(true);
            while(true)
                Console.ReadLine();
        }
    }


}

/*TO Do: item drops, skill books
 *
 * Game Loop
 * An enemy spawns - A player kills mob - An item drops - Player Takes & Uses item
 * Progress: 
 * As player gets stronger, it kills stronger opponents, gets more interesting items & (ability books, magic items, legendary weapons)
 * Minigames:-fishing
 *
 scienario:

village enviroment
-Forest(x trees) //grows in number over time, can be cut down for wood resource, some wood might be collected without cutting trees, from falling branches
-Mountain(x meters) //mine depth at mountain, ores might be acquired with pickaxe, ore quality might increase with depth, rocks might be collected without tool
Sea Shore & River // fishing spot with some richness
npc villagers //for trading, mating and companionship
buildings  // some neutral, some made by player for resting,marriage,crafting,defence,farming,cooking etc
other biomes and maybe weathers, may vary from village to village

crafting

stone club,wood shield,wood bow
opal staff

bronze sword(bandit&craft) bronze axe, bronze pickaxe,bronze knife
emerald staff,recurve bow

iron equipments, diamond staff, compound bow

titanium equipments, uranium staff, magnetic bow

attributes

stats
str: increases physical damage by 1 and hp by 1
agi: increases defence by 1 and stamina by 1 
wis: increase mana by 1

str: get better equipment and hulk smash
agi: spam abilities, be untouchable
wis: cast spells, do the impossible

abilities
damage bonus abiliteis, damage multiplier abilities, true damage abilities, health percent damage abilities
poison abilities,
hurting hit(+5 damage), eye hit(2x agi), weak poison hit(10 damage over time), blood loss(%1 health)
gap hit(%25 ignore armor) nerve hit(+20 damage)

spells
damaging spells, necromancy, heal, buff, curse, item conversion & produce, botanic, weather, summoning

skill acquisition: if player reaches the learning stats, skill is automaticly received

mobs:
wolf
bandit
zombie
goblin
witch
ogre
mage
black knight
dragon

passive:    they breed but may damage forest or grassland in large numbers
sheep
rabbit


versioning

- basic mob spawning and combat, basic items
- boss summoning
- spellcasting and other abilities
- crafting,forestry,fishing,farming,hunting and mining
- buildings,marriage,companionship,weathers

defining details
item tree
 */