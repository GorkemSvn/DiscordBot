using System;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
namespace DiscordBot
{
    public class Commands
    {
        [ Summary("Lists commands and their explanations")]
        public void Help(SocketCommandContext context)
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
            emb.Author.WithName(context.Message.Author.Username) ;
            emb.WithImageUrl($"attachment://sen-wu-viking02.jpg");
            emb.Description = help;
            var embed = emb.Build();
            context.Channel.SendFileAsync("C:/Users/User/Desktop/sen-wu-viking02.jpg", null,false, embed);

            Console.WriteLine("Helping the user");
        }
    }
}
