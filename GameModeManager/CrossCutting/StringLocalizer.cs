// Included libraries
using Microsoft.Extensions.Localization;

// Declare namespace
namespace GameModeManager.CrossCutting
{
    // Define class
    public class StringLocalizer
    {
        // Define class dependencies
        private readonly string _prefix;
        private IStringLocalizer _localizer;

        // Define class constructors
        public StringLocalizer(IStringLocalizer localizer)
        {
            _localizer = localizer;
            _prefix = "plugin.prefix";
        }

        public StringLocalizer(IStringLocalizer localizer, string prefix)
        {
            _prefix = prefix;
            _localizer = localizer;
        }

        // Define class methods
        public string LocalizeWithPrefixInternal(string prefix, string key, params object[] args)
        {
            return $"{_localizer[prefix]} {Localize(key, args)}";
        }

        public string LocalizeWithPrefix(string key, params object[] args)
        {
            return LocalizeWithPrefixInternal(_prefix, key, args);
        }

        public string Localize(string key, params object[] args)
        {
            return _localizer[key, args];
        }
    }
}