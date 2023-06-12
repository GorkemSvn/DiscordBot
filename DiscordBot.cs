﻿using System;
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
    public class Bot
    {
        DiscordSocketClient client;

        string token = "NjU1NzE5MTcyNDYxODIxOTUz.GslYx8.rHJF6GWeDj2YP641BgxOoWz9kevpnqvQ4q_SOk";
        Commands commands;
        public Bot()
        {
            RunBotAsync();
        }
        public async Task RunBotAsync()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
                GatewayIntents = GatewayIntents.All
            });
            client.Connected += OnConnect;
            client.LoggedIn += OnLogIn;

            await client.LoginAsync(Discord.TokenType.Bot, token);
            await client.StartAsync();
            client.MessageReceived += HandleCommandAsync;
            commands = new Commands();
            await Task.Delay(1);

        }


        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            var context = new SocketCommandContext(client, message);

            if (message.Author.IsBot)
                return;

            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                List<string> words = new List<string>();
                words.AddRange(message.Content.Split(' '));
                string command = words[0];
                words.RemoveAt(0);
                command = command.TrimStart('!');


                var method = commands.GetType().GetMethod(command);
                if (method != null)
                {
                    var parameters = new List<object>();
                    parameters.Add(context);

                    if (words.Count > 0)
                        parameters.Add(words);

                    method.Invoke(commands, parameters.ToArray());
                    Console.WriteLine(method.Name);
                }
            }
        }

        async Task OnLogIn()
        {
            Console.WriteLine("Logged in");
        }
        async Task OnConnect()
        {
            Console.WriteLine("Connected");
        }
    }
}