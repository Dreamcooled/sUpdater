using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using SUpdater.Model;
using SUpdater.Utils;

namespace SUpdater
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        private readonly List<object> _selectedEpisodeTreeItems = new List<object>();

        public MainWindow()
        {
            InitializeComponent();
            TreeViewExtensions.SetSelectedItems(ShowTreeView, _selectedEpisodeTreeItems);
            TreeViewExtensions.AddSelectionChangedListener(ShowTreeView, _selectedEpisodeTreeItems_CollectionChanged);


            Entity show1 = new Entity("Big Bang Theory",EntityType.Show);
            show1.Init();
            DataContext = show1;

          /*  Console.WriteLine("init done");
            Console.WriteLine("Title Loaded: "+show1["Title"].Loaded);
            Console.WriteLine("Homepage Loaded: "+show1["Homepage"].Loaded);
            Console.WriteLine("Cover Loaded: "+show1["Cover"].Loaded);

            Console.WriteLine("Cover String:" + show1["Cover"].String);
            Console.WriteLine("Cover Loaded: " + show1["Cover"].Loaded);
            var x = show1["Cover"].StringData;*/
        }

        private void _selectedEpisodeTreeItems_CollectionChanged(object sender)
        {
            var first = _selectedEpisodeTreeItems.FirstOrDefault() as Entity;
            if (_selectedEpisodeTreeItems.Count == 1 && first.Type==EntityType.Season)
            {
                EpisodeTabControl_Season.DataContext = first;
                EpisodeTabControl.SelectedIndex = 1;
            }
            else if (_selectedEpisodeTreeItems.Count == 1 && first.Type==EntityType.Episode)
            {
                EpisodeTabControl_Episode.DataContext = first;
                EpisodeTabControl.SelectedIndex = 2;
            }
            else if (!_selectedEpisodeTreeItems.Any())
            {
                EpisodeTabControl_Episode.DataContext = _selectedEpisodeTreeItems;
                EpisodeTabControl.SelectedIndex = 0;
            }
            else
            {
                EpisodeTabControl.SelectedIndex = 3;
            }
        }

    }
}
