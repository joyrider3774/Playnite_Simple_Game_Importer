using Newtonsoft.Json;
using Playnite.SDK;
using System.Collections.Generic;

namespace SimpleGameImport
{
    public class SimpleGameImportSettings : ISettings
    {
        private readonly SimpleGameImport plugin;

        public int DefaultDuplicateDetectionIndex { get; set; } = 2;
        
        private SimpleGameImportSettings EditDataSettings;
        
        // Parameterless constructor must exist if you want to use LoadPluginSettings method.
        public SimpleGameImportSettings()
        {
        }

        public SimpleGameImportSettings(SimpleGameImport plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<SimpleGameImportSettings>();

            // LoadPluginSettings returns null if not saved data is available.
            if (savedSettings != null)
            {
                RestoreSettings(savedSettings);
            }
        }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
            EditDataSettings = new SimpleGameImportSettings(plugin);
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
            RestoreSettings(EditDataSettings);
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            plugin.SavePluginSettings(this);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }

        private void RestoreSettings(SimpleGameImportSettings source)
        {
            DefaultDuplicateDetectionIndex = source.DefaultDuplicateDetectionIndex;

        }
    }
}