﻿using System;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BitchSlapperBot
{
    class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;

        String token = "MzE1NDk5MTY1OTA2MTA4NDE3.DLBXfA.RZWZfiPmodrKZfhgLTsZEKqPwX8";
        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();

            await InstallCommands();
            
            client.UserJoined += UserJoined;

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            await client.SetGameAsync("Slap Simulator");

            client.Log += Log;
            await Task.Delay(-1);
        }

        public async Task InstallCommands()
        {
            // Hook the MessageReceived Event into our Command Handler
            client.MessageReceived += HandleCommand;
            // Discover all of the commands in this assembly and load them.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task UserJoined (SocketGuildUser user)
        {
            var channel = client.GetChannel(363007801821364226) as SocketTextChannel;
            await channel.SendMessageAsync("Hi" + user.Mention + "!!!");

        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            // Create a Command Context
            var context = new CommandContext(client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await commands.ExecuteAsync(context, argPos);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }



        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.WhenAll();
        }
    }
}
