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
        static void Main(string[] args)
        {
            var bot = new Bot();
            Server.Load();
            Server.SetActive(true);
            while(true)
                Console.ReadLine();
        }
    }


}

/*
 scienario:

village enviroment
Forest(x trees) //grows in number over time, can be cut down for wood resource, some wood might be collected without cutting trees, from falling branches
Mountain(x meters) //mine depth at mountain, ores might be acquired with pickaxe, ore quality might increase with depth, rocks might be collected without tool
Sea Shore & River // fishing spot with some richness
npc villagers //for trading, mating and companionship
buildings  // some neutral, some made by player for resting,marriage,crafting,defence,farming,cooking etc
other biomes and maybe weathers, may vary from village to village

crafting
stone axe,pickaxe,wood shield,wood bow
quarts staff
bronze sword(bandit&craft) bronze axe, bronze pickaxe,bronze knife
opal staff,crossbow
iron equipments, Gold staff, rifle
titanium equipments, diamond staff, m4

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

killer takes item
 */