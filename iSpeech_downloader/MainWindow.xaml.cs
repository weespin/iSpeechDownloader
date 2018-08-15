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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iSpeech_downloader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded+=AnimatingControl_Loaded;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AnimatingControl_Loaded(object sender, RoutedEventArgs e)
        {
          
        }

        private void Woman_OnClick(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (this.FindResource("ToLeft") as Storyboard);
            sb.Begin();
        }

        private void Man_OnClick(object sender, RoutedEventArgs e)
        {
            Storyboard sb = (this.FindResource("ToRight") as Storyboard);
            sb.Begin();
        }
    }
}
