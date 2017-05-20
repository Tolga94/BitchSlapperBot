using System;
using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;

namespace BitchSlapperBot
{
    class MyBot
    {
        // Attributes
        DiscordClient discord;
        CommandService commands;
        Dictionary<string, int> slapUsers = new Dictionary<string, int>();


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


            getSlapUsers();
            commands = discord.GetService<CommandService>();
            slap();

            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("MzE1NDk5MTY1OTA2MTA4NDE3.DAHtkg.LQTkR30o4TC7F5WOYO1GBcYh_hk", TokenType.Bot);
                discord.SetGame("Slapping Simulator");
            });
        }

        private void slap()
        {
            /**
            commands.CreateCommand("hello")
               .Do(async (e) =>
               {
                   await e.Channel.SendMessage("Hi, " + e.User.Name.ToString());
               });**/

            commands.CreateCommand("slap").Parameter("user", ParameterType.Required).Do(async (e) =>
            {
                string user = e.Args[0];
                Console.WriteLine(user);
                var expr = @"[<][@]\d+[>]$";
                Match match = Regex.Match(user, expr);
                Console.WriteLine(match.Value);
                Console.WriteLine(match.Success);
                string message = "bloop";
                if (match.Success)
                { 

                    if (slapUsers.ContainsKey(user))
                    {
                        var count = slapUsers[user];
                        slapUsers[user] = count + 1;
                        saveSlapUsers();
                    }
                    else
                    {
                        slapUsers.Add(user, 1);
                        saveSlapUsers();
                    }

                    if (user.Equals("<@279276010640506880>"))
                    {
                        message = "Tut mir Leid, " + e.User.Name + ". Ich bin nicht autorisiert meinen Schöpfer Snake zu slappen.";
                    }
                    else
                    {
                        message = "SLAP!!! "+ user + " wurde gerade ge-bitch-slapped, weil er/sie etwas Dämliches gesagt oder getan hat.\nAnzahl Slaps: "+ slapUsers[user]+".";
                    }
                }
                else
                {
                    message = "Der Spieler " + user + " wurde nicht gefunden. Versuch's doch mal mit einem Mention (@Name)!";
                }
                await e.Channel.SendMessage(message);

                
            });
        }

        private void getSlapUsers()
        {
            Dictionary<string, int> dict = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(@"..\..\slaps.json"));
            slapUsers = dict;
        }

        private void saveSlapUsers()
        {
            File.WriteAllText(@"..\..\slaps.json", JsonConvert.SerializeObject(slapUsers));
        }

        private void Log(object Sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

    }
}
