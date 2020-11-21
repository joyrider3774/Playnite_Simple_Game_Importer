using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace SimpleGameImport
{
    public class SimpleGameImport : Plugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private static IResourceProvider resources = new ResourceProvider();

        private string pluginFolder;

        private SimpleGameImportSettings Settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("6048b590-bdfb-4a88-8953-2985b4f5c3dc");

        public SimpleGameImport(IPlayniteAPI api) : base(api)
        {
            Settings = new SimpleGameImportSettings(this);

            pluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Localization.SetPluginLanguage(pluginFolder, api.ApplicationSettings.Language);
        }

        public void DoImportGames()
        {
            GameImportView ImportView = new GameImportView(this);
            var window = PlayniteApi.Dialogs.CreateWindow(new WindowCreationOptions
            {
                ShowMinimizeButton = false,
            });

            window.Height = 350;
            window.Width = 650;
            window.Title = "Simple Game Importer";

            // Set content of a window. Can be loaded from xaml, loaded from UserControl or created from code behind
            window.Content = ImportView;

            // Set owner if you need to create modal dialog window
            window.Owner = PlayniteApi.Dialogs.GetCurrentAppWindow();
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Use Show or ShowDialog to show the window
            window.ShowDialog();
        }

        public override List<MainMenuItem> GetMainMenuItems(GetMainMenuItemsArgs args)
        {
            List<MainMenuItem> MainMenuItems = new List<MainMenuItem>
            {
                new MainMenuItem {
                    MenuSection = "@Simple Game Importer",
                    Icon = Path.Combine(pluginFolder, "icon.png"),
                    Description = resources.GetString("LOC_SIMPLEGAMEIMPORTER_Import"),
                    Action = (gameMenuItem) =>
                    {
                       DoImportGames();                        
                    }
                }
             };
            return MainMenuItems;
        }


        public override void OnGameInstalled(Game game)
        {
            // Add code to be executed when game is finished installing.
        }

        public override void OnGameStarted(Game game)
        {
            // Add code to be executed when game is started running.
        }

        public override void OnGameStarting(Game game)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameStopped(Game game, long elapsedSeconds)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameUninstalled(Game game)
        {
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted()
        {
            // Add code to be executed when Playnite is initialized.
        }

        public override void OnApplicationStopped()
        {
            // Add code to be executed when Playnite is shutting down.
        }

        public override void OnLibraryUpdated()
        {
            // Add code to be executed when library is updated.
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return Settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new ImportCSVSettingsView();
        }
    }
}