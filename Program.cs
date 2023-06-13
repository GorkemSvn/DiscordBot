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
