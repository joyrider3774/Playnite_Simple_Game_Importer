﻿using Playnite.SDK;
using Playnite.SDK.Events;
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
    public class SimpleGameImport : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private static readonly IResourceProvider resources = new ResourceProvider();

        private string PluginFolder { get; set; }

        private SimpleGameImportSettingsViewModel Settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("6048b590-bdfb-4a88-8953-2985b4f5c3dc");

        public SimpleGameImport(IPlayniteAPI api) : base(api)
        {
            Settings = new SimpleGameImportSettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };

            PluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Localization.SetPluginLanguage(PluginFolder, api.ApplicationSettings.Language);
        }

        public void DoImportGames()
        {
            try
            {
                GameImportView ImportView = new GameImportView(this, Settings);
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
            catch (Exception E)
            {
                logger.Error(E, "Error during initializing GameImportView");
                PlayniteApi.Dialogs.ShowErrorMessage(E.Message, "Error during DoImportGames");
            }
        }

        public override IEnumerable<MainMenuItem> GetMainMenuItems(GetMainMenuItemsArgs args)
        {
            List<MainMenuItem> MainMenuItems = new List<MainMenuItem>
            {
                new MainMenuItem {
                    MenuSection = "@Simple Game Importer",
                    Icon = Path.Combine(PluginFolder, "icon.png"),
                    Description = resources.GetString("LOC_SIMPLEGAMEIMPORTER_Import"),
                    Action = (MainMenuItem) =>
                    {
                       DoImportGames();                        
                    }
                }
             };
            return MainMenuItems;
        }


        public override void OnGameInstalled(OnGameInstalledEventArgs args)
        {
            // Add code to be executed when game is finished installing.
        }

        public override void OnGameStarted(OnGameStartedEventArgs args)
        {
            // Add code to be executed when game is started running.
        }

        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameUninstalled(OnGameUninstalledEventArgs args)
        {
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            // Add code to be executed when Playnite is initialized.
        }

        public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
        {
            // Add code to be executed when Playnite is shutting down.
        }

        public override void OnLibraryUpdated(OnLibraryUpdatedEventArgs args)
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