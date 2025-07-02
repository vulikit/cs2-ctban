using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API;

namespace cs2_ctban
{
    public partial class ctban
    {
        public void reply(CCSPlayerController pl, string m)
        {
            pl.PrintToChat(prefix + m);
        }

        public void broadcast(string m)
        {
            foreach (var item in Utilities.GetPlayers())
            {
                reply(item, m);
            }
        }

        private TargetResult? GetTarget(CommandInfo command)
        {
            var matches = command.GetArgTargetResult(1);

            if (!matches.Any())
            {
                command.ReplyToCommand(prefix + Localizer["NoPerson"]);
                return null;
            }

            if (command.GetArg(1).StartsWith('@'))
                return matches;

            if (matches.Count() == 1)
                return matches;

            command.ReplyToCommand(prefix + Localizer["MoreThanOnePerson"]);
            return null;
        }
    }
}
