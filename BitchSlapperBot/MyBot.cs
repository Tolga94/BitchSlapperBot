using System;
using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitchSlapperBot
{
    class MyBot
    {
        // Attributes
        DiscordClient discord;
        CommandService commands;


        // Constructor
        public MyBot()
        {
            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });


            discord.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
            });

            

            commands = discord.GetService<CommandService>();
            slap();

            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("MzE1NDk5MTY1OTA2MTA4NDE3.DAHtkg.LQTkR30o4TC7F5WOYO1GBcYh_hk", TokenType.Bot);
            });

            discord.SetGame("Slapping Simulator 2017");
            discord.SetStatus("Slapping Simulator 2017");
        }

        private async void slap()
        {
            commands.CreateCommand("hello")
               .Do(async (e) =>
               {
                   await e.Channel.SendMessage("Hi, " + e.User.Name.ToString());
               });

            commands.CreateCommand("slap").Parameter("user", ParameterType.Required).Do(async (e) =>
            {
                var user = e.Args[0];
                string message = "bloop";
                if (user.ToLower().Equals("snake"))
                {
                    message = "Tut mir Leid, " + e.User.Name + ". Ich bin nicht autorisiert meinen Schöpfer Snake zu slappen.";
                }
                else
                {
                    message = user + " wurde gerade ge-bitch-slapped, weil er/sie etwas Dämliches gesagt oder getan hat.";
                }

                await e.Channel.SendMessage(message);

                
            });
        }

        private void Log(object Sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

    }
}
