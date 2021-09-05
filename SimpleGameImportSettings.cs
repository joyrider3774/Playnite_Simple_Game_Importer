using Newtonsoft.Json;
using Playnite.SDK;
using Playnite.SDK.Data;
using System.Collections.Generic;

namespace SimpleGameImport
{

    public class SimpleGameImportSettings
    {
        public int DefaultDuplicateDetectionIndex { get; set; } = 2;
    }

    public class SimpleGameImportSettingsViewModel : ObservableObject, ISettings
    {
        private readonly SimpleGameImport plugin;
        private SimpleGameImportSettings editingClone { get; set; }

        private SimpleGameImportSettings settings;
        public SimpleGameImportSettings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }

        public SimpleGameImportSettingsViewModel(SimpleGameImport plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<SimpleGameImportSettings>();

            // LoadPluginSettings returns null if not saved data is available.
            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new SimpleGameImportSettings();
            }
        }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
            editingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
            Settings = editingClone;
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            plugin.SavePluginSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }
    }
}