// Included libraries
using GameModeManager.Core;
using WASDMenuAPI.Shared.Models;
using GameModeManager.CrossCutting;
using CounterStrikeSharp.API.Modules.Menu;

// Declare namespace
namespace GameModeManager.Menus
{
    // Define class
    public class RTVMenus
    {
        // Define class dependencies
        public WasdMenuController WasdMenus;
        public BaseMenuController BaseMenus;

        // Define class instance
        public RTVMenus(PluginState pluginState, StringLocalizer localizer, VoteManager voteManager, Config config)
        {
            WasdMenus = new WasdMenuController(pluginState, localizer, voteManager);
            BaseMenus = new BaseMenuController(pluginState, localizer, voteManager, config);
        }

        // Define WasdMenuController class
        public class WasdMenuController(PluginState pluginState, StringLocalizer localizer, VoteManager voteManager)
        {
            // Define class properties
            public IWasdMenu? MainMenu;
            private MenuFactory menuFactory = new MenuFactory();

            // Define load method
            public void Load(List<string> options)
            {
                pluginState.RTV.Votes.Clear();
                MainMenu = menuFactory.WasdMenus.AssignMenu(localizer.Localize("rtv.hud.menu-title"));

                foreach (var optionName in options)
                {
                    pluginState.RTV.Votes[optionName] = 0;
                    MainMenu?.Add(optionName, (player, option) =>
                    {
                        voteManager.AddVote(player, optionName);
                        menuFactory.WasdMenus.CloseMenu(player);
                    });
                }
            }
        }

        // Define BaseMenuController class
        public class BaseMenuController(PluginState pluginState, StringLocalizer localizer, VoteManager voteManager, Config config)
        {
            // Define class properties
            private MenuFactory menuFactory = new MenuFactory();
            public BaseMenu MainMenu = new ChatMenu(localizer.Localize("rtv.hud.menu-title"));

            // Define load method
            public void Load(List<string> options)
            {
                pluginState.RTV.Votes.Clear();
                MainMenu = menuFactory.BaseMenus.AssignMenu(config.RTV.Style, localizer.Localize("rtv.hud.menu-title"));

                foreach (var optionName in options.Take(config.RTV.OptionsToShow))
                {
                    pluginState.RTV.Votes[optionName] = 0;
                    MainMenu.AddMenuOption(optionName, (player, option) =>
                    {
                        voteManager.AddVote(player, optionName);
                        menuFactory.BaseMenus.CloseMenu(player);
                    });
                }
            }
        }
    }
}