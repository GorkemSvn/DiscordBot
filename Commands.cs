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

                help += line+"\n";
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
        public async void Profile(SocketMessage sMessage)
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

            var dmc = await sMessage.Author.CreateDMChannelAsync();
            dmc.SendMessageAsync(null, false, embed);
        }

        [Summary("Shows bjects around the village ")]
        public void Enviroment(SocketMessage sMessage)
        {
            var character = Operations.GetCharacter(sMessage);

            var objs = character.village.GetPool();

            var builder = new EmbedBuilder();
            builder.Title =sMessage.Channel.Name+ " Village :";
            foreach (var item in objs)
            {
                builder.Description += item.GetType().Name + " " + item.name + "\n";
            }
            sMessage.Channel.SendMessageAsync(null, false, builder.Build());
        }


        public void Attack(SocketMessage sMessage,List<string> names)
        {
            var character = Operations.GetCharacter(sMessage);

            Village village = character.village;
            var enviroment = village.GetPool();
            string targetName = "";

            if (names.Count > 1)
            {
                for (int i = 0; i < names.Count-1; i++)
                {
                    targetName += names[i]+" ";
                }
                targetName += names[names.Count - 1];
            }
            else
                targetName = names[0];

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

        public void Logs(SocketMessage sMessage)
        {
            var character = Operations.GetCharacter(sMessage);

            character?.village.SendLogs();
        }

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
        }
    }

    public class PrivateCommands
    {

    }
}
