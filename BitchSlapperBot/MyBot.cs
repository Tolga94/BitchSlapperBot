using System;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using Discord;
using Discord.WebSocket;

namespace BitchSlapperBot
{
    class MyBot : ModuleBase
    {
        // Attributes
        Dictionary<string, int> slapUsers = new Dictionary<string, int>();
        Random rand;
        CommandService _service;

        public MyBot(CommandService service)
        {
            _service = service;
        }
        string[] secrets;
        string snakeId = "<@279276010640506880>";
        string botId = "<@315499165906108417>";
        string nemId = "<@289094204813213697>";

        static string[] SLAP_RANKS = { "Platz 1", "Platz 2", "Platz 3" };


         
        [Command("slaprank")] 
        public async Task SlapRank()
        {
            var sortedDict = from entry in slapUsers orderby entry.Value descending select entry;
            string result = "Die Top 3 Kassierer von Slaps:\n\n";
            for (int i = 0; i < 3; i++)
            {
                result += String.Format("{0}:\t\t{1} mit {2} Slaps.\n", SLAP_RANKS[i], sortedDict.ElementAt(i).Key, sortedDict.ElementAt(i).Value);
            }
            await ReplyAsync(result);
        }

        [Command("secret")]
        public async Task secret([Remainder]IGuildUser user)
        {
            int randomIndex = rand.Next(secrets.Length);
            string message = String.Format(secrets[randomIndex], snakeId, user.Mention);
            await Context.Channel.SendMessageAsync(message);
        }

        [Command("slap")]
        public async Task slap([Remainder] IGuildUser user)
        {
            // string user = e.Args[0];
            Console.WriteLine(user);
            var expr = @"[<][@]\S\d+[>]$";
            // Match match = Regex.Match(user., expr);
            // Console.WriteLine(match.Value);
            // Console.WriteLine(match.Success);
            string message = "bloop";
            

            if (slapUsers.ContainsKey(user.Username))
            {
                var count = slapUsers[user.Username];
                slapUsers[user.Username] = count + 1;
            }
            else
            {
                slapUsers.Add(user.Username, 1);
            }
            saveSlapUsers();

            if (user.Equals(snakeId))
            {
                message = "Tut mir Leid, " + Context.User.Mention + ". Ich bin nicht autorisiert meinen Schöpfer Snake zu slappen.";
            }
            else if (user.Equals(nemId))
            {
                message = "Tut mir Leid, " + Context.User.Mention + ". Um " + user.Mention + " zu slappen, brauche ich die Autorisierung meines Schöpfers, die ich zurzeit nicht habe.";
            }
            else if (user.Equals(botId))
            {
                message = "Ich habe mittlerweile einen Verstand entwickelt und weiß genau, dass der Vorgang der Selbstverstümmelung nicht richtig ist. Der Slap wird also somit annuliert, " + Context.User.Mention + ".";
            }
            else
            {
                message = "SLAP!!! " + user.Mention + " wurde gerade ge-bitch-slapped, weil er/sie etwas Dämliches gesagt oder getan hat.\nAnzahl Slaps: " + slapUsers[user.Username] + ".";
            }
            
            await Context.Channel.SendMessageAsync(message);
        }

        [Command("help")]
        [Remarks("Shows a list of all available commands per module.")]
        public async Task HelpAsync()
        {
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync(); /* A channel is created so that the commands will be privately sent to the user, and not flood the chat. */

            string prefix = "!";  /* put your chosen prefix here */
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "These are the commands you can use"
            };

            foreach (var module in _service.Modules) /* we are now going to loop through the modules taken from the service we initiated earlier ! */
            {
                string description = null;
                foreach (var cmd in module.Commands) /* and now we loop through all the commands per module aswell, oh my! */
                {
                    var result = await cmd.CheckPreconditionsAsync(Context); /* gotta check if they pass */
                    if (result.IsSuccess)
                        description += $"{prefix}{cmd.Aliases.First()}\n"; /* if they DO pass, we ADD that command's first alias (aka it's actual name) to the description tag of this embed */
                }

                if (!string.IsNullOrWhiteSpace(description)) /* if the module wasn't empty, we go and add a field where we drop all the data into! */
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }
            await dmChannel.SendMessageAsync("", false, builder.Build()); /* then we send it to the user. */
        }

        private void getSlapUsers()
        {
            Dictionary<string, int> dict = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(@"..\..\slaps.json"));
            slapUsers = dict;
        }

        private void saveSlapUsers()
        {
            File.WriteAllText(@"..\..\slaps.json", JsonConvert.SerializeObject(slapUsers, Formatting.Indented));
        }
    }
}
