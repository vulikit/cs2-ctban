using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Numerics;

namespace cs2_ctban
{
    public partial class ctban : BasePlugin, IPluginConfig<s03>
    {
        public override string ModuleName => "cs2-ctban";
        public override string ModuleVersion => "0.0.3s";
        public override string ModuleAuthor => "varkit";

        public s03 Config { get; set; }
        private string prefix { get; set; }
        private List<CTBanPlayer> _bannedPlayers = new List<CTBanPlayer>();
        private string _jsonFilePath => Path.Combine(ModuleDirectory, "ctbans.json");

        public void OnConfigParsed(s03 config)
        {
            Config = config;
            prefix = config.Prefix.ReplaceColorTags();
            LoadBansFromJson();
        }

        public override void Load(bool hotReload)
        {
            base.Load(hotReload);
            RegisterEventHandler<EventPlayerTeam>(OnPlayerTeam);
            RegisterEventHandler<EventRoundStart>(OnRoundStart);
        }

        private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            LoadBansFromJson();
            SaveBansToJson();
            return HookResult.Continue;
        }

        [ConsoleCommand("ctban")]
        public void Command_CTBan(CCSPlayerController? caller, CommandInfo command)
        {
            if (!Config.CTBanPermmissions.Any(p => AdminManager.PlayerHasPermissions(caller, p)))
            {
                reply(caller, Localizer["NoPermission"]);
                return;
            }

            if (command.ArgCount < 2 || command.ArgCount > 3)
            {
                reply(caller, Localizer["WrongUsage"]);
                return;
            }

            int sure = 0;
            DateTime timetoexpire = DateTime.MaxValue;

            if (command.ArgCount == 3)
            {
                if (!int.TryParse(command.ArgByIndex(2), out sure))
                {
                    reply(caller, Localizer["EnterNumber"]);
                    return;
                }
                if (sure <= 0)
                {
                    reply(caller, Localizer["NumberCanNot"]);
                    return;
                }
                timetoexpire = DateTime.Now.AddMinutes(sure);
            }

            var targets = GetTarget(command);
            if (targets == null) return;
            var playersToTarget = targets.Players.Where(player => player.IsValid && player is { IsBot: false, IsHLTV: false }).ToList();
            if (playersToTarget.Count != 1)
            {
                reply(caller, Localizer["SpecifyOnePerson"]);
                return;
            }

            playersToTarget.ForEach(player =>
            {
                BanPlayer(player, timetoexpire, caller);
                Console.WriteLine($"CTBan | {Localizer["Time"]}: {(sure == 0 ? Localizer["Unlimited"] : sure + Localizer["Minutes"])}");
            });
        }

        [ConsoleCommand("ctunban")]
        public void Command_CTUnban(CCSPlayerController? caller, CommandInfo command)
        {
            if (!Config.CTBanPermmissions.Any(p => AdminManager.PlayerHasPermissions(caller, p)))
            {
                reply(caller, Localizer["NoPermission"]);
                return;
            }

            var targets = GetTarget(command);
            if (targets == null || targets.Players.Count != 1)
            {
                reply(caller, Localizer["SpecifyOnePerson"]);
                return;
            }

            var player = targets.Players.First();
            if (_bannedPlayers.RemoveAll(b => b.SteamID == player.SteamID) > 0)
            {
                SaveBansToJson();
                broadcast(Localizer["UnbannedBroadcast", player.PlayerName, caller.PlayerName]);
                reply(caller, Localizer["UnbannedSuccesfully", player.PlayerName]);
            }
            else
            {
                reply(caller, Localizer["PlayerIsntBannedFromCT"]);
            }
        }

        public void BanPlayer(CCSPlayerController madur, DateTime bitmezaman, CCSPlayerController yetkili)
        {
            if (madur == null || !madur.IsValid || madur.SteamID == 0) return;

            var ban = new CTBanPlayer
            {
                SteamID = madur.SteamID,
                BanExpireDate = bitmezaman,
                BanYetkiliIsim = yetkili?.PlayerName ?? "Console",
                BanYetkiliSteamID = yetkili?.SteamID ?? 0
            };

            _bannedPlayers.RemoveAll(b => b.SteamID == madur.SteamID);
            _bannedPlayers.Add(ban);
            SaveBansToJson();

            reply(madur, Localizer["YouAreBannedFromCT", (bitmezaman == DateTime.MaxValue ? Localizer["Unlimited"] : $"{Localizer["Time"]}: {bitmezaman.Subtract(DateTime.Now).TotalMinutes:F0} {Localizer["Minutes"]}")]);
            broadcast(Localizer["BannedFromCTBroadcast", madur.PlayerName, yetkili.PlayerName, (bitmezaman == DateTime.MaxValue ? Localizer["Unlimited"] : $"{Localizer["Time"]}: {bitmezaman.Subtract(DateTime.Now).TotalMinutes:F0} {Localizer["Minutes"]}")]);

            if (madur.Team == CsTeam.CounterTerrorist)
            {
                madur.SwitchTeam(CsTeam.Terrorist);
            }
        }

        private void LoadBansFromJson()
        {
            try
            {
                if (File.Exists(_jsonFilePath))
                {
                    var json = File.ReadAllText(_jsonFilePath);
                    _bannedPlayers = JsonSerializer.Deserialize<List<CTBanPlayer>>(json) ?? new List<CTBanPlayer>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading CT bans from JSON: {ex.Message}");
            }
        }

        private void SaveBansToJson()
        {
            try
            {
                _bannedPlayers.RemoveAll(b => b.BanExpireDate != DateTime.MaxValue && b.BanExpireDate <= DateTime.Now);
                var json = JsonSerializer.Serialize(_bannedPlayers, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_jsonFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving CT bans to JSON: {ex.Message}");
            }
        }

        public bool IsPlayerCTBanned(CCSPlayerController player)
        {
            if (player == null || !player.IsValid || player.SteamID == 0) return false;
            var ban = _bannedPlayers.FirstOrDefault(b => b.SteamID == player.SteamID);
            if (ban == null) return false;
            if (ban.BanExpireDate == DateTime.MaxValue || ban.BanExpireDate > DateTime.Now)
            {
                return true;
            }
            _bannedPlayers.Remove(ban);
            SaveBansToJson();
            return false;
        }

        private HookResult OnPlayerTeam(EventPlayerTeam @event, GameEventInfo info)
        {
            var player = @event.Userid;
            if (player == null || !player.IsValid || player.SteamID == 0) return HookResult.Continue;

            if (@event.Team == (int)CsTeam.CounterTerrorist && IsPlayerCTBanned(player))
            {
                Server.NextFrame(() =>
                {
                    player.SwitchTeam(CsTeam.Terrorist);
                    reply(player, Localizer["YouCannotJoinCT"]);
                });
                return HookResult.Handled;
            }

            return HookResult.Continue;
        }
    }

    public class CTBanPlayer
    {
        public ulong SteamID { get; set; }
        public DateTime BanExpireDate { get; set; }
        public string BanYetkiliIsim { get; set; }
        public ulong BanYetkiliSteamID { get; set; }
    }
}