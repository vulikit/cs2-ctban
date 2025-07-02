# cs2-ctban

A Counter-Strike 2 plugin for banning players from joining the Counter-Terrorist (CT) team, built using the CounterStrikeSharp API.

## Features
- Ban players from joining the CT team for a specified duration or permanently.
- Unban players from CT restrictions.
- Automatically switch banned players to the Terrorist team if they attempt to join CT.
- Persist bans across sessions using a JSON file (`ctbans.json`).
- Reload bans and remove expired ones at the start of each round.
- Multilingual support with localization (e.g., English, Russian, Turkish).
- Permission-based access for ban and unban commands.

## Installation
1. Ensure you have [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) installed on your CS2 server.
2. Download the latest release of `cs2-ctban` from the [Releases](https://github.com/vulikit/cs2-ctban/releases) page.
3. Extract the plugin files to the `counterstrikesharp/plugins` folder in your server directory.
4. Restart your server or load the plugin manually using `css_plugin load cs2-ctban`.

## Usage
### Commands
- `!ctban <player> [duration(minutes)]`  
  Bans a player from joining the CT team. Specify a duration in minutes for a temporary ban or omit for a permanent ban.  
  Example: `!ctban PlayerName 60` (bans for 60 minutes) or `!ctban PlayerName` (permanent ban).
- `!ctunban <player>`  
  Removes the CT ban from a player.  
  Example: `!ctunban PlayerName`.

### Notes
- Only one player can be targeted per command.
- Bots and HLTV clients cannot be banned.
- Bans are saved to `ctbans.json` in the plugin's directory and reloaded at the start of each round.
- Expired bans are automatically removed during round start.

## Configuration
The plugin uses a configuration file (`cs2-ctban.json`) to define settings such as the chat prefix and permissions. Place the configuration file in the `counterstrikesharp/configs/plugins/cs2-ctban` folder.

### Example Configuration
```json
{
  "Prefix": "{green}[CTBan] {white}",
  "CTBanPermmissions": ["@css/ban"]
}
```

- **Prefix**: Customizable chat prefix for plugin messages, supporting color tags.
- **CTBanPermmissions**: List of permissions required to use `!ctban` and `!ctunban` commands (e.g., `@css/ban`).

#### Example `en.json`
```json
{
  "WrongUsage": "{red}Incorrect usage! {white}Use: !ctban <player> <duration(minutes)>",
  "EnterNumber": "{red}Please enter a valid number",
  "NumberCanNot": "{red}Duration cannot be zero or negative. Leave blank for a permanent ban",
  "SpecifyOnePerson": "{red}Please target exactly one player",
  "PlayerIsntBannedFromCT": "{red}This player is not banned from CT!",
  "UnbannedBroadcast": "{green}{0}'s {white}CT ban was removed by {green}{1}!",
  "UnbannedSuccesfully": "{green}{0}'s {white}CT ban was successfully removed!",
  "YouAreBannedFromCT": "{red}You are banned from CT! {0}",
  "BannedFromCTBroadcast": "{green}{0} {white}was banned from CT by {green}{1} {white}for {green}{2}!",
  "YouCannotJoinCT": "{red}You are banned from CT and cannot join!",
  "Unlimited": "Permanent",
  "Time": "Duration",
  "Minutes": "minutes",
  "NoPerson": "{red}Player not found",
  "MoreThanOnePerson": "{red}Multiple players found",
  "NoPermission": "{red}You do not have permission."
}
```

## Permissions
- The plugin checks for permissions defined in `CTBanPermmissions` (e.g., `@css/ban`) for both `!ctban` and `!ctunban` commands.
- Admins must have the specified permissions to execute these commands.

## Requirements
- Counter-Strike 2 server.
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) framework.

## Contributing
Contributions are welcome! Feel free to submit issues or pull requests on the [GitHub repository](https://github.com/vulikit/cs2-ctban).

## Credits
- **Author**: varkit
- **Version**: 0.0.3s
