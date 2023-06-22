using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using System.Threading.Tasks;
using System.Reflection;
using Discord.WebSocket;
using Discord;
using Rpg;

namespace DiscordBot
{
    public class Commands
    {
        //These are the commands player can cast from group channels

        [ Summary("Lists commands and their explanations")]
        public void Help(SocketMessage sMessage)
        {
            var rawMethods = typeof(Commands).GetMethods();
            List<MethodInfo> methods = new List<MethodInfo>();
            methods.AddRange(rawMethods);
            methods.RemoveRange(methods.Count - 4, 4);

            var title = "Here's the commands you can use :\n \n";
            var help = "\n";
            foreach (var m in methods)
            {
                var ats = m.GetCustomAttributes(true);
                string line = "!" + m.Name+"\n";
                if (ats.Length > 0 && ats[0] is SummaryAttribute)
                    line += (ats[0] as SummaryAttribute).Text;

                help += line+"\n \n";
            }

            var emb = new EmbedBuilder();
            emb.Title = title;
            emb.Author = new EmbedAuthorBuilder();
            emb.Author.WithName(sMessage.Author.Username) ;
            //emb.WithImageUrl($"attachment://sen-wu-viking02.jpg");
            emb.Description = help;
            var embed = emb.Build();
            sMessage.Channel.SendMessageAsync( null,false, embed);
            
            Console.WriteLine("Helping the user");
        }

        [Summary("Shows player's information")]
        public void Profile(SocketMessage sMessage)
        {
            var character = Operations.GetCharacter(sMessage);

            var embedBuilder = new EmbedBuilder();

            embedBuilder.Title = "Profile";

            embedBuilder.Author = new EmbedAuthorBuilder();
            embedBuilder.Author.WithName(sMessage.Author.Username);
            embedBuilder.Author.WithIconUrl(sMessage.Author.GetAvatarUrl());

            embedBuilder.Description ="Health :"+ character.stats.health.energy+"/"+ character.stats.health.capasity;
            embedBuilder.Description +="\n Stamina :" + character.stats.stamina.energy + "/" + character.stats.stamina.capasity;
            embedBuilder.Description += "\n Mana :" + character.stats.mana.energy + "/" + character.stats.mana.capasity;
            embedBuilder.Description += "\n Strenght :" + character.stats.strenght.level+" ("+character.stats.strenght.exp+" exp)";
            embedBuilder.Description += "\n Agility :" + character.stats.agility.level + " (" + character.stats.agility.exp+" exp)";
            embedBuilder.Description += "\n Wisdom :" + character.stats.wisdom.level + " (" + character.stats.wisdom.exp+" exp)";
            embedBuilder.Description += "\n Physical Defence :" + character.equipments.physicalDefence;
            embedBuilder.Description += "\n Magical Defence :" + character.equipments.magicalDefence;
            var embed = embedBuilder.Build();

            sMessage.Channel.SendMessageAsync(null, false, embed);
        }

        [Summary("Shows event logs ")]
        public void Logs(SocketMessage sMessage)
        {
            var character = Operations.GetCharacter(sMessage);

            character?.village.SendLogs();
        }

        [Summary("(!SetName Juan Rick) Set your character's name (limited usage)")]
        public void SetName(SocketMessage sMessage,List<string> names)
        {
            var character = Operations.GetCharacter(sMessage);
            character.name = Operations.CombineWords(names);
            sMessage.Channel.SendMessageAsync("Name changed to " + character.name);
        }


        #region Interactions
        [Summary("(!Attack Evil Monster) Attacks target character ")]
        public void Attack(SocketMessage sMessage,List<string> names)
        {
            var character = Operations.GetCharacter(sMessage);

            Village village = character.village;
            var enviroment = village.GetPool();
            string targetName = Operations.CombineWords(names);

            foreach (var item in enviroment)
            {
                if(item.name==targetName && item is Character)
                {
                    character.Attack(item as Character);
                    sMessage.Channel.SendMessageAsync("A fight begins!");
                    return;
                }
            }
        }
        [Summary("Shows objects around the village ")]
        public void Enviroment(SocketMessage sMessage)
        {
            var character = Operations.GetCharacter(sMessage);

            var objs = character.village.GetPool();

            var builder = new EmbedBuilder();
            builder.Title = sMessage.Channel.Name + " Village :";
            foreach (var item in objs)
            {
                builder.Description += " " + item.name + "\n";
            }
            sMessage.Channel.SendMessageAsync(null, false, builder.Build());
        }
        /*[Summary("Cuts some tree from forest. ")]
        public void Timber(SocketMessage sMessage)
        {
            var character = Operations.GetCharacter(sMessage);

            var pool=character?.village.GetPool();

            foreach (var item in pool)
            {
                if(item is Forest)
                {
                    var forest = item as Forest;
                    var wood = forest.Cut();
                    if (wood != null)
                    {
                        character.inventory.Add(wood);
                        sMessage.Channel.SendMessageAsync("Wood added to your inventory");

                        return;
                    }
                }
            }

                sMessage.Channel.SendMessageAsync("There were no trees left");
        }
        [Summary("Try to mine ores at mountan mine. ")]
        public void Mine(SocketMessage sMessage)
        {
            var character = Operations.GetCharacter(sMessage);

            var pool=character?.village.GetPool();

            foreach (var item in pool)
            {
                if(item is Mine)
                {
                    var mine = item as Mine;
                    var ore = mine.Dig();
                    if (ore != null)
                    {
                        character.inventory.Add(ore);
                        sMessage.Channel.SendMessageAsync(ore.name +" added to your inventory");

                        return;
                    }
                }
            }

                sMessage.Channel.SendMessageAsync("There were no mine nearby");
        }
        */
        [Summary("'!Ability Name' to use ability ")]
        public void Ability(SocketMessage sMessage, List<string> names)
        {
            if (names.Count < 2)//1 ability name, rest is target name
                return;

            var abilityName = names[0];

            names.RemoveAt(0);

            var targetName = Operations.CombineWords(names);
            Character crc = Operations.GetCharacter(sMessage);
            var target = Operations.FindObect(targetName,crc.village);
            var abilty = crc.skills.Find(abilityName);

            if (target != null && abilty !=null)
            {
                sMessage.Channel.SendMessageAsync("Using " + abilty.name + " on " + targetName);
                abilty.Perform(target);
            }
        }
        #endregion

        #region Inventory 
        [Summary("(Equip Black Sword) Equip named item and discard already equiped one")]
        public void Equip(SocketMessage sMessage, List<string> names)
        {
            var character = Operations.GetCharacter(sMessage);
            var items = character.inventory.GetVirtualList();
            var eqpm = Operations.CombineWords(names);

            foreach (var item in items)
            {
                if (item.name == eqpm && item is Equipments.Equipment equipment)
                {
                    character.equipments.EquipAndGetDiscarded(equipment);

                    sMessage.Channel.SendMessageAsync(equipment.name +" equiped!");
                    return;
                }
            }
            sMessage.Channel.SendMessageAsync("Could not find "+eqpm );
        }
        [Summary("(Equip Black Sword) Equip named item and discard already equiped one")]
        public void Use(SocketMessage sMessage, List<string> names)
        {
            var character = Operations.GetCharacter(sMessage);
            var items = character.inventory.GetVirtualList();
            var name = Operations.CombineWords(names);
            var item = Operations.FindItem(name, character.inventory);
            if (item != null)
            {
                item.Use(character);

                sMessage.Channel.SendMessageAsync(name +" is used");
            }
            else
                sMessage.Channel.SendMessageAsync("Could not find " + name);
        }
        [Summary("!Collect Apple - Take dropped item")]
        public void Collect(SocketMessage sMessage, List<string> names)
        {
            var character = Operations.GetCharacter(sMessage);
            var eqpm = Operations.CombineWords(names);

            var targt = Operations.FindObect(eqpm, character.village);
            if (targt != null && targt is ItemCapsule container)
            {
                character.inventory.Add(container.GiveItem());
                sMessage.Channel.SendMessageAsync(eqpm+" added to your inventory");
            }
            else
                sMessage.Channel.SendMessageAsync("Could not find " + eqpm);
        }

        [Summary("See what you got. ")]
        public void Inventory(SocketMessage sMessage)
        {
            var character = Operations.GetCharacter(sMessage);
            var items = character.inventory.GetVirtualList();
            var embedBuilder = new EmbedBuilder();
            embedBuilder.Title = "Inventory";
            for (int i = 0; i < items.Count; i++)
            {
                if (i % 2 == 0)
                {
                    embedBuilder.Description += "\n "  + "- " + items[i].name + " (" + items[i].quantity + ")";
                }
                else
                {
                    embedBuilder.Description += "    "  + "- " + items[i].name + " (" + items[i].quantity + ")";
                }
            }
            sMessage.Channel.SendMessageAsync(null, false, embedBuilder.Build());
        }
        [Summary("See what you got. ")]
        public void Abilities(SocketMessage sMessage)
        {
            var character = Operations.GetCharacter(sMessage);
            var items = character.skills.GiveVirtualList();
            var embedBuilder = new EmbedBuilder();
            embedBuilder.Title = "Abilities";
            for (int i = 0; i < items.Count; i++)
            {
                if (i % 2 == 0)
                {
                    embedBuilder.Description += "\n "  + "- " + items[i].name + " (" + items[i].power.level + ")";
                }
                else
                {
                    embedBuilder.Description += "    "  + "- " + items[i].name + " (" + items[i].power.level + ")";
                }
            }
            sMessage.Channel.SendMessageAsync(null, false, embedBuilder.Build());
        }
        [Summary("See what you can craft. ")]
        public void Craftables(SocketMessage sMessage)
        {
            var character = Operations.GetCharacter(sMessage);
            var craftables = Crafting.CheckCraftables(character.inventory);

            var embedBuilder = new EmbedBuilder();
            embedBuilder.Title = "Craftables";
            for (int i = 0; i < craftables.Count; i++)
            {
                if (i % 2 == 0)
                {
                    embedBuilder.Description += "\n " + i + "- " + craftables[i].craft.name ;
                }
                else
                {
                    embedBuilder.Description += "    " + i + "- " + craftables[i].craft.name;
                }
            }
            sMessage.Channel.SendMessageAsync(null, false, embedBuilder.Build());
        }
        [Summary("(!Craft Black Sword) Name to craft item")]
        public void Craft(SocketMessage sMessage, List<string> names)
        {
            var character = Operations.GetCharacter(sMessage);
            var targetItem = Operations.CombineWords(names);
            var craftable = Crafting.GetRecipe(targetItem);

            if (craftable != null)
            {
                if (craftable.Craft(character.inventory))
                    sMessage.Channel.SendMessageAsync(craftable.craft.name + " has been crafted");
                else
                    sMessage.Channel.SendMessageAsync("Not enough ingredients for " + craftable.craft.name);

                return;
            }

            sMessage.Channel.SendMessageAsync("Could not find recipe for " + targetItem+ ", please check your craftables");
        }
        #endregion


        static class Operations
        {
            public static Hero GetCharacter(SocketMessage sMessage)
            {
                var author = sMessage.Author;

                //get character if already there is
                Hero character = null;
                if (Server.players.ContainsKey(author.Id))
                    character = Server.players[author.Id];

                //create character if its not registered
                else
                {
                    character = new Hero(author.Id, author.Username);
                    Server.players.Add(author.Id, character);
                }


                //check channel and correct his world
                var channel = sMessage.Channel;
                if(channel.GetChannelType()== ChannelType.Text)
                {
                    if (Server.villages.ContainsKey(channel.Id))
                    {
                        var world = Server.villages[channel.Id];
                        character.SetVillage(world);
                    }
                    else
                    {
                        var newWorld = new Village(channel.Id);
                        Server.villages.Add(newWorld.id, newWorld);
                        character.SetVillage(newWorld);
                    }
                }


                return character;
            }
            public static string CombineWords(List<string> words)
            {
                var line = words[0];

                if (words.Count > 1)
                {
                    for (int i = 1; i < words.Count; i++)
                    {
                        line += " "+ words[i];
                    }
                }
                return line;
            }
            public static object FindObect(string name, Village village)
            {
                var enviroment = village.GetPool();

                foreach (var item in enviroment)
                {
                    if (item.name == name)
                    {
                        return item;
                    }
                }
                return null;
            }
            public static Item FindItem(string name,Inventory inventory)
            {
                var items = inventory.GetVirtualList();
                foreach (var item in items)
                {
                    if (item.name == name)
                    {
                        return item;
                    }
                }
                return null;
            }
        }
    }

    public class PrivateCommands
    {

    }
}
