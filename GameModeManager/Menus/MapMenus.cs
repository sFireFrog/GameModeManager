// Included libraries
using GameModeManager.Core;
using CounterStrikeSharp.API;
using WASDMenuAPI.Shared.Models;
using GameModeManager.CrossCutting;
using GameModeManager.Shared.Models;
using CounterStrikeSharp.API.Modules.Menu;

// Declare namespace
namespace GameModeManager.Menus
{
    // Define class
    public class MapMenus
    {
        // Define class dependencies
        public WasdMenuController WasdMenus;
        public BaseMenuController BaseMenus;

        // Define class instance
        public MapMenus(PluginState pluginState, StringLocalizer localizer, ServerManager serverManager, Config config)
        {
            WasdMenus = new WasdMenuController(config, pluginState, localizer, serverManager);
            BaseMenus = new BaseMenuController(config, pluginState, localizer, serverManager);
        }

        // Define WasdMenuController class
        public class WasdMenuController(Config config, PluginState pluginState, StringLocalizer localizer, ServerManager serverManager)
        {
            // Define class properties
            public IWasdMenu? MainMenu;
            public IWasdMenu? VoteMenu;
            private MenuFactory menuFactory = new MenuFactory();

            // Define load method  
            public void Load()
            {
                if (config.Maps.Mode == 1)
                {
                    // Create main menu
                    MainMenu = menuFactory.WasdMenus.AssignMenu(localizer.Localize("maps.menu-title"));

                    foreach (IMap _map in pluginState.Game.Maps)
                    {
                        MainMenu?.Add(_map.DisplayName, (player, option) =>
                        {
                            menuFactory.WasdMenus.CloseMenu(player);
                            Server.PrintToChatAll(localizer.LocalizeWithPrefix("changemap.message", player.PlayerName, _map.Name));
                            serverManager.ChangeMap(_map, config.Maps.Delay);
                        });
                    }

                    // Create vote menu
                    VoteMenu = menuFactory.WasdMenus.AssignMenu(localizer.Localize("maps.menu-title"));

                    foreach (IMap _map in pluginState.Game.Maps)
                    {
                        VoteMenu?.Add(_map.DisplayName, (player, option) =>
                        {
                            menuFactory.WasdMenus.CloseMenu(player);
                            CustomVoteManager.CustomVotesApi.Get()?.StartCustomVote(player, _map.Name);
                        });
                    }
                }
                else if (config.Maps.Mode == 0)
                {
                    // Create main menu
                    MainMenu = menuFactory.WasdMenus.AssignMenu(localizer.Localize("maps.menu-title"));

                    foreach (IMap _map in pluginState.Game.CurrentMode.Maps)
                    {
                        MainMenu?.Add(_map.DisplayName, (player, option) =>
                        {
                            menuFactory.WasdMenus.CloseMenu(player);
                            Server.PrintToChatAll(localizer.LocalizeWithPrefix("changemap.message", player.PlayerName, _map.Name));
                            serverManager.ChangeMap(_map, config.Maps.Delay);
                        });
                    }

                    // Create vote menu
                    VoteMenu = menuFactory.WasdMenus.AssignMenu(localizer.Localize("maps.menu-title"));

                    foreach (IMap _map in pluginState.Game.CurrentMode.Maps)
                    {
                        // Add menu option
                        VoteMenu?.Add(_map.DisplayName, (player, option) =>
                        {
                            menuFactory.WasdMenus.CloseMenu(player);
                            CustomVoteManager.CustomVotesApi.Get()?.StartCustomVote(player, _map.Name);
                        });
                    }
                }
            }
        }

        // Define BaseMenuController class
        public class BaseMenuController(Config config, PluginState pluginState, StringLocalizer localizer, ServerManager serverManager)
        {
            // Define class properties
            private MenuFactory menuFactory = new MenuFactory();
            public BaseMenu MainMenu = new ChatMenu(localizer.Localize("maps.menu-title"));
            public BaseMenu VoteMenu = new ChatMenu(localizer.Localize("maps.menu-title"));

            // Define load method
            public void Load()
            {
                if (config.Maps.Mode == 1)
                {
                    // Create main menu
                    MainMenu = menuFactory.BaseMenus.AssignMenu(config.Maps.Style, localizer.Localize("modes.menu-title"));

                    foreach (IMode _mode in pluginState.Game.Modes)
                    {
                        MainMenu.AddMenuOption(_mode.Name, (player, option) =>
                        {
                            // Create sub menu
                            BaseMenu subMenu = menuFactory.BaseMenus.AssignMenu(config.Maps.Style, localizer.Localize("maps.menu-title"));

                            foreach (IMap _map in _mode.Maps)
                            {
                                subMenu.AddMenuOption(_map.DisplayName, (player, option) =>
                                {
                                    Server.PrintToChatAll(localizer.LocalizeWithPrefix("changemap.message", player.PlayerName, _map.Name));
                                    menuFactory.BaseMenus.CloseMenu(player);
                                    serverManager.ChangeMap(_map, config.Maps.Delay);
                                });
                            }
                            // Open sub menu
                            menuFactory.BaseMenus.OpenMenu(subMenu, player);
                        });
                    }

                    // Create vote menu
                    VoteMenu = menuFactory.BaseMenus.AssignMenu(config.Maps.Style, localizer.Localize("modes.menu-title"));

                    foreach (IMode _mode in pluginState.Game.Modes)
                    {
                        VoteMenu.AddMenuOption(_mode.Name, (player, option) =>
                        {
                            // Create sub menu
                            BaseMenu subMenu = menuFactory.BaseMenus.AssignMenu(config.Maps.Style, localizer.Localize("maps.menu-title"));

                            foreach (IMap _map in _mode.Maps)
                            {
                                subMenu.AddMenuOption(_map.DisplayName, (player, option) =>
                                {
                                    menuFactory.BaseMenus.CloseMenu(player);
                                    CustomVoteManager.CustomVotesApi.Get()?.StartCustomVote(player, _map.Name);
                                });
                            }
                            // Open sub menu
                            menuFactory.BaseMenus.OpenMenu(subMenu, player);
                        });
                    }
                }
                else if (config.Maps.Mode == 0)
                {
                    // Create main menu
                    MainMenu = menuFactory.BaseMenus.AssignMenu(config.Maps.Style, localizer.Localize("maps.menu-title"));

                    foreach (IMap _map in pluginState.Game.CurrentMode.Maps)
                    {
                        MainMenu.AddMenuOption(_map.DisplayName, (player, option) =>
                        {
                            menuFactory.BaseMenus.CloseMenu(player);
                            Server.PrintToChatAll(localizer.LocalizeWithPrefix("changemap.message", player.PlayerName, _map.Name));
                            serverManager.ChangeMap(_map, config.Maps.Delay);
                        });
                    }

                    // Create vote menu
                    VoteMenu = menuFactory.BaseMenus.AssignMenu(config.Maps.Style, localizer.Localize("maps.menu-title"));

                    foreach (IMap _map in pluginState.Game.CurrentMode.Maps)
                    {
                        VoteMenu.AddMenuOption(_map.DisplayName, (player, option) =>
                        {
                            menuFactory.BaseMenus.CloseMenu(player);
                            CustomVoteManager.CustomVotesApi.Get()?.StartCustomVote(player, _map.Name);
                        });
                    }
                }
            }
        }
    }
}