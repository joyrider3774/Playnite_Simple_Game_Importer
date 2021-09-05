using System;
using System.Collections.Generic;
using System.Linq;
using Playnite.SDK.Models;
using System.Windows;
using System.Windows.Controls;
using Playnite.SDK;

namespace SimpleGameImport
{
    public partial class GameImportView : UserControl
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private readonly SimpleGameImport plugin;
        private static readonly IResourceProvider resources = new ResourceProvider();
        public List<string> PlatformsList { get; set; } = new List<string>();
        public List<string> SourcesList { get; set; } = new List<string>();

        public GameImportView()
        {
            InitializeComponent();
        }
        public GameImportView(SimpleGameImport plugin, SimpleGameImportSettingsViewModel Settings)
        {
            try
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
                CmbDuplicateDetection.SelectedIndex = Settings.Settings.DefaultDuplicateDetectionIndex;
            }
            catch (Exception E)
            {
                logger.Error(E, "Error during initializing GameImportView");
                plugin.PlayniteApi.Dialogs.ShowErrorMessage(E.Message, "Error during initializing GameImportView");
            }
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
            try
            {
                var gamesText = tbGameNames.Text.Replace("\r\n", "\n");
                var games = gamesText.Split('\n');
                foreach (string game in games)
                {
                    if ((game.Trim() != ""))
                    {
                        bool Exists = false;
                        if (CmbDuplicateDetection.SelectedIndex != 0)
                        {

                            bool NameMatched = false;
                            bool PlatformMatched = false;
                            bool SourceMatched = false;
                            bool PlatformSourceMatched = false;
                            var GameSearch = plugin.PlayniteApi.Database.Games.Where(o => o.Name == game);
                            if (GameSearch.HasItems())
                            {
                                NameMatched = true;
                                string PlatformName = CmbPlatforms.SelectedItem.ToString();
                                string SourceName = CmbSources.SelectedItem == null ? "" : CmbSources.SelectedItem.ToString();

                                foreach (Game PlayniteGame in GameSearch)
                                {
                                    bool tmpPlatformMatched = false;
                                    if (PlayniteGame.Platforms != null)
                                    {
                                        foreach (Platform p in PlayniteGame.Platforms)
                                        {
                                            if (p.Name.Equals(PlatformName))
                                            {
                                                tmpPlatformMatched = true;
                                                PlatformMatched = true;
                                                break;
                                            }
                                        }
                                    }

                                    string PlayniteSourceName = PlayniteGame.Source == null ? "" : PlayniteGame.Source.Name;
                                    if (PlayniteSourceName == SourceName)
                                    {
                                        SourceMatched = true;
                                    }

                                    if (tmpPlatformMatched &&
                                        (PlayniteSourceName == SourceName) ||
                                        //in case no source selected check on platform only
                                        ((SourceName == "") &&
                                        tmpPlatformMatched))
                                    {
                                        PlatformSourceMatched = true;
                                    }
                                }
                            }
                            Exists = ((CmbDuplicateDetection.SelectedIndex == 1) && NameMatched) ||
                                ((CmbDuplicateDetection.SelectedIndex == 2) && NameMatched && PlatformMatched) ||
                                ((CmbDuplicateDetection.SelectedIndex == 3) && NameMatched && SourceMatched) ||
                                ((CmbDuplicateDetection.SelectedIndex == 4) && NameMatched && PlatformSourceMatched);
                        }

                        if (Exists)
                        {
                            NumGamesNotAddedDouble++;
                            continue;
                        }

                        var newGame = new Game(game);
                        var platformList = plugin.PlayniteApi.Database.Platforms.Where(o => o.Name == CmbPlatforms.SelectedItem.ToString());
                        if (platformList.HasItems())
                        {
                            List<Guid> PlatformIds = new List<Guid>();
                            PlatformIds.Add(platformList.First().Id);
                            newGame.PlatformIds = PlatformIds;
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
                plugin.PlayniteApi.Dialogs.ShowMessage(resources.GetString("LOC_SIMPLEGAMEIMPORTER_GamesAdded") + $" {NumGamesAdded}\n" +
                resources.GetString("LOC_SIMPLEGAMEIMPORTER_GamesNotAdded") + $" {NumGamesNotAddedDouble} ");
            }
            catch (Exception E)
            {
                logger.Error(E, "Error during Adding of games");
                plugin.PlayniteApi.Dialogs.ShowErrorMessage(E.Message, "Error during Adding of games");
            }           
        }
    }
}
