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
using SUpdater.Model;

namespace SUpdater
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Entity show1 = new Entity("Big Bang Theory",EntityType.Show);
            show1.Init();
            DataContext = show1;
          /*  Console.WriteLine("init done");
            Console.WriteLine("Title Loaded: "+show1["Title"].Loaded);
            Console.WriteLine("Homepage Loaded: "+show1["Homepage"].Loaded);
            Console.WriteLine("Cover Loaded: "+show1["Cover"].Loaded);

            Console.WriteLine("Cover Data:" + show1["Cover"].Data);
            Console.WriteLine("Cover Loaded: " + show1["Cover"].Loaded);
            var x = show1["Cover"].Data;*/
        }
    }
}
