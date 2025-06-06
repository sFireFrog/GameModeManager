// Included libraries
using GameModeManager.Menus;
using GameModeManager.Models;
using CounterStrikeSharp.API;
using WASDMenuAPI.Shared.Models;
using GameModeManager.Contracts;
using CounterStrikeSharp.API.Core;
using GameModeManager.CrossCutting;
using GameModeManager.Shared.Models;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

// Declare namespace
namespace GameModeManager.Features
{
    // Define class
    public class MapCommands : IPluginDependency<Plugin, Config>
    {
        // Define class dependencies
        private MapMenus _mapMenus;
        private PluginState _pluginState;
        private StringLocalizer _localizer;
        private ServerManager _serverManager;
        private Config _config = new Config();
        private MenuFactory _menuFactory = new MenuFactory();

        // Define class instance
        public MapCommands(PluginState pluginState, StringLocalizer localizer, ServerManager serverManager)
        {
            _localizer = localizer;
            _pluginState = pluginState;
            _serverManager = serverManager;
            _mapMenus = new MapMenus(pluginState, localizer, serverManager, _config);
        }

        // Load config
        public void OnConfigParsed(Config config)
        {
            _config = config;
        }

        // Define on load behavior
        public void OnLoad(Plugin plugin)
        {
            if (_config.Commands.Map)
            {
                plugin.AddCommand("css_map", "Changes the map to the map specified in the command argument.", OnMapCommand);
            }
            if (_config.Commands.Maps)
            {
                plugin.AddCommand("css_maps", "Displays a list of maps from the current mode.", OnMapsCommand);
            }
        }

        // Define admin map menu command handler
        [RequiresPermissions("@css/changemap")]
        [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
        public void OnMapsCommand(CCSPlayerController? player, CommandInfo command)
        {
            if(player != null)
            {
                if (_config.Maps.Style.Equals("wasd"))
                {
                    _mapMenus.WasdMenus.Load();
                    IWasdMenu? menu =_mapMenus.WasdMenus.MainMenu;

                    if(menu != null)
                    {
                        _menuFactory.WasdMenus.OpenMenu(player, menu);
                    }
                }
                else
                {
                    _mapMenus.BaseMenus.Load();
                    BaseMenu menu = _mapMenus.BaseMenus.MainMenu;
                    _menuFactory.BaseMenus.OpenMenu(menu, player);
                }
            }
        }

        // Define admin change map command handler
        [RequiresPermissions("@css/changemap")]
        [CommandHelper(minArgs: 1, usage: "<map name> optional: <workshop id>", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
        public void OnMapCommand(CCSPlayerController? player, CommandInfo command)
        {
            IMap _newMap = new Map($"{command.ArgByIndex(1)}",$"{command.ArgByIndex(2)}");
            IMap? _foundMap = _pluginState.Game.Maps.FirstOrDefault(g => g.Name.Equals($"{command.ArgByIndex(1)}", StringComparison.OrdinalIgnoreCase) || g.WorkshopId.ToString().Equals("{command.ArgByIndex(2)}", StringComparison.OrdinalIgnoreCase));

            if (_foundMap != null)
            {
                _newMap = _foundMap; 
            }

            if(player != null)
            {
                Server.PrintToChatAll(_localizer.LocalizeWithPrefix("changemap.message", player.PlayerName, _newMap.Name));
            }
            
            _serverManager.ChangeMap(_newMap, _config.Maps.Delay);
        }
    }
}