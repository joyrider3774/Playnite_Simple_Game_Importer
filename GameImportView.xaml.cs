using System;
using System.Collections.Generic;
using System.Linq;
using Playnite.SDK.Models;
using System.Windows;
using System.Windows.Controls;
using Playnite.SDK;

namespace SimpleGameImport
{
    public partial class GameImportView: UserControl
    {
        private readonly SimpleGameImport plugin;
        private static IResourceProvider resources = new ResourceProvider();
        public List<string> PlatformsList { get; set; } = new List<string>();
        public List<string> SourcesList { get; set; } = new List<string>();
        
        public GameImportView()
        {
            InitializeComponent();
        }
        public GameImportView(SimpleGameImport plugin)
        {
            this.plugin = plugin;            
            var TmpPlatformsList = plugin.PlayniteApi.Database.Platforms.AsQueryable().OrderBy(o => o.Name).ToList();
            foreach (Platform platform in TmpPlatformsList)
            {
                PlatformsList.Add(platform.Name);
            }
            var TmpSourcesList = plugin.PlayniteApi.Database.Sources.AsQueryable().OrderBy(o => o.Name).ToList();
            SourcesList.Add("");
            foreach (GameSource source in TmpSourcesList)
            {
                SourcesList.Add(source.Name);
            }
            InitializeComponent();
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int NumGamesAdded = 0;
            int NumGamesNotAddedDouble = 0;
            if (tbGameNames.Text.Trim() == "")
            {
                plugin.PlayniteApi.Dialogs.ShowMessage(resources.GetString("LOC_SIMPLEGAMEIMPORTER_NoGames"));
                return;
            }
            if (CmbPlatforms.SelectedItem == null)
            {
                plugin.PlayniteApi.Dialogs.ShowMessage(resources.GetString("LOC_SIMPLEGAMEIMPORTER_NoPlatform"));
                return;
            }
            var gamesText = tbGameNames.Text.Replace("\r\n", "\n");
            var games = gamesText.Split('\n');
            foreach (string game in games)
            {
                if ((game.Trim() != ""))
                {
                    bool Exists = false;
                    var GameSearch = plugin.PlayniteApi.Database.Games.Where(o => o.Name == game);
                    if (GameSearch.HasItems())
                    {
                        foreach(Game PlayniteGame in GameSearch)
                        {
                            if (PlayniteGame.Platform.Name == CmbPlatforms.SelectedItem.ToString())
                            {
                                Exists = true;
                                NumGamesNotAddedDouble++;
                                break;
                            }
                        }
                    }

                    if (!Exists)
                    {
                        var newGame = new Game(game);
                        var platformList = plugin.PlayniteApi.Database.Platforms.Where(o => o.Name == CmbPlatforms.SelectedItem.ToString());
                        if (platformList.HasItems())
                        {
                            newGame.PlatformId = platformList.First().Id;
                        }
                        if (CmbSources.SelectedItem != null && (CmbSources.SelectedItem.ToString() != ""))
                        {
                            var sourceList = plugin.PlayniteApi.Database.Sources.Where(o => o.Name == CmbSources.SelectedItem.ToString());
                            if (sourceList.HasItems())
                            {
                                newGame.SourceId = sourceList.First().Id;
                            }
                        }
                        newGame.Added = DateTime.Now;
                        plugin.PlayniteApi.Database.Games.Add(newGame);
                        NumGamesAdded++;
                    }
                }                
            }
            plugin.PlayniteApi.Dialogs.ShowMessage(resources.GetString("LOC_SIMPLEGAMEIMPORTER_GamesAdded") + $": {NumGamesAdded}\n" +
                resources.GetString("LOC_SIMPLEGAMEIMPORTER_GamesNotAdded") +  $": {NumGamesNotAddedDouble} ");
        }
    }
}
