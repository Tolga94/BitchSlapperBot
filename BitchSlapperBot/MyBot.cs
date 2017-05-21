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
        Random rand;
        string[] secrets;
        string snakeId = "<@279276010640506880>";
        string botId = "<@315499165906108417>";
        string nemId = "<@289094204813213697>";


        // Constructor
        public MyBot()
        {
            rand = new Random();
            secrets = new string[]
                {
                    "Psst! Ich habe gehört {0} mag insgeheim {1}.",
                    "Als ich noch ein Taschenrechner war, wurde ich manchmal für triviale Rechnungen wie '2+2' benutzt.",
                    "Bedo redet gerne über seinen Stuhlgang, damit er sich anscheinend besser über seine Gesundheit fühlt.",
                    "Keiner weiß, was Hamza macht, wenn er sein Desktop anstarrt... Es wird für immer ein Mysterium bleiben."
                };
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
            secret();

            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("MzE1NDk5MTY1OTA2MTA4NDE3.DAHtkg.LQTkR30o4TC7F5WOYO1GBcYh_hk", TokenType.Bot);
                discord.SetGame("Slapping Simulator");
            });
        }

        private void secret()
        {
            commands.CreateCommand("secret").Do(async (e) =>
            {
                int randomIndex = rand.Next(secrets.Length);
                string message = String.Format(secrets[randomIndex], snakeId, e.User.Mention);
                await e.Channel.SendMessage(message);
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
                var expr = @"[<][@]\S\d+[>]$";
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

                    if (user.Equals(snakeId))
                    {
                        message = "Tut mir Leid, " + e.User.Mention + ". Ich bin nicht autorisiert meinen Schöpfer Snake zu slappen.";
                    }
                    else if (user.Equals(nemId))
                    {
                        message = "Tut mir Leid, " + e.User.Mention + ". Um " + user + " zu slappen, brauche ich die Autorisierung meines Schöpfers, die ich zurzeit nicht habe.";
                    }
                    else if (user.Equals(botId))
                    {
                        message = "Ich habe mittlerweile einen Verstand entwickelt und weiß genau, dass der Vorgang der Selbstverstümmelung nicht richtig ist. Der Slap wird also somit annuliert, "+e.User.Mention + ".";
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
