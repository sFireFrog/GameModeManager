// Included libraries
using CounterStrikeSharp.API;
using GameModeManager.Contracts;
using CounterStrikeSharp.API.Core;
using GameModeManager.CrossCutting;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;
using TimerFlags = CounterStrikeSharp.API.Modules.Timers.TimerFlags;
 
// Declare namespace
namespace GameModeManager.Core
{
    // Define class
    public class RotationManager : IPluginDependency<Plugin, Config>
    {
        // Define class dependencies
        bool _rotationEnabled = false;
        private Timer? _rotationTimer;
        private PluginState _pluginState;
        private ServerManager _serverManager;
        private Config _config = new Config();

        // Define class constructor
        public RotationManager(ServerManager serverManager, PluginState pluginState)
        {
            _pluginState = pluginState;
            _serverManager = serverManager;
        }
        
        // Load config
        public void OnConfigParsed(Config config)
        {
            _config = config;
            _pluginState.Game.RotationsEnabled = _config.Rotation.Enabled;
        }

        // Define on load behavior
        public void OnLoad(Plugin plugin)
        { 
            // Register event handlers
            plugin.RegisterEventHandler<EventCsWinPanelMatch>(EventCsWinPanelMatchHandler);
            plugin.RegisterEventHandler<EventPlayerDisconnect>(EventPlayerDisconnectHandler);
            plugin.RegisterEventHandler<EventPlayerConnectFull>(EventPlayerConnectFullHandler);

            // Create mode schedules
            if (_config.Rotation.ModeSchedules)
            {
                foreach (ScheduleEntry entry in _config.Rotation.Schedule)
                {
                    DateTime targetTime = DateTime.Parse(entry.Time);
                    TimeSpan delay = targetTime - DateTime.Now;
                    if (delay.TotalMilliseconds <= 0)
                    {
                        delay = delay.Add(TimeSpan.FromDays(1));
                    }

                    new Timer((float)delay.TotalSeconds, () =>
                    {
                        _serverManager.TriggerScheduleChange(entry);
                        delay = targetTime.AddDays(1) - DateTime.Now;  
                        
                    },TimerFlags.REPEAT);
                }
            }
        }   

        // Define event handlers
        public HookResult EventCsWinPanelMatchHandler(EventCsWinPanelMatch @event, GameEventInfo info)
        {  
            _serverManager.TriggerRotation();
            return HookResult.Continue;
        }

        public HookResult EventPlayerConnectFullHandler(EventPlayerConnectFull @event, GameEventInfo info)
        {
            if (PlayerExtensions.ValidPlayerCount(false) > 0)
            {
                _rotationTimer?.Kill();
            }
            return HookResult.Continue;
        }

        public HookResult EventPlayerDisconnectHandler(EventPlayerDisconnect @event, GameEventInfo info)
        {  
            if(_config.Rotation.WhenServerEmpty)
            {
                if(ServerExtensions.IsServerEmpty())
                {
                    // Disable server hibernation
                    if(!ServerExtensions.IsHibernationEnabled())
                    {
                        Server.ExecuteCommand("sv_hibernate_when_empty false");
                    }

                    // Create timer
                    if(!_rotationEnabled)
                    {
                        _rotationEnabled = true;
                        _rotationTimer = new Timer(_config.Rotation.CustomTimeLimit, () =>
                        {
                            _serverManager.TriggerRotation();
                            _rotationEnabled = false;
                        },TimerFlags.REPEAT);
                    }
                }
            }
            return HookResult.Continue;
        }
    }
}